# Dukaan.Media Service

Media asset management microservice. Handles chunked file uploads, image processing (WebP conversion, resizing), and object storage via MinIO.

## Responsibility

- Chunked file upload protocol (init, upload chunks, complete)
- Image processing: converts uploads to WebP format, generates 3 size variants (original, display, thumbnail)
- Object storage in MinIO (S3-compatible)
- Metadata management in PostgreSQL
- Background processing via Hangfire (chunk combining, image processing, cleanup)
- Presigned URL generation for direct MinIO access

**Does NOT handle:** product/category management, business domain objects. Only manages media assets and exposes storage paths/URLs.

## Ports

| Context | Port |
|---------|------|
| Docker (external) | 5002 |
| Docker (internal) | 8080 |
| Local dev | 5148 (HTTP), 7262 (HTTPS) |
| Hangfire Dashboard | /hangfire |

## API Endpoints

All endpoints under `/api/media`.

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/media/chunk/init` | JWT | Initiate chunked upload. Returns `mediaId`, `totalChunks`, `chunkSize` (5 MB) |
| POST | `/api/media/chunk/{mediaId}/{chunkIndex}` | JWT | Upload single chunk (multipart form) |
| POST | `/api/media/chunk/{mediaId}/complete` | JWT | Signal upload complete. Enqueues `ProcessImageJob` |
| GET | `/api/media/{id}` | Anonymous | Poll media status. Returns 202 if uploading, 200 with metadata if completed |
| GET | `/api/media/{id}/url?variant=display` | JWT | Get 1-hour presigned MinIO URL for variant |
| DELETE | `/api/media/{id}` | JWT | Delete media from storage and database |

### Response DTOs

```csharp
// InitiateUploadResponse
record(Guid MediaId, int TotalChunks, long ChunkSize, string Message)

// UploadChunkResponse
record(Guid MediaId, int ChunkIndex, int UploadedChunks, int TotalChunks, MediaStatus Status)

// CompleteUploadResponse
record(Guid MediaId, MediaStatus Status, string Message)

// MediaMetadataResponse
record(Guid Id, string OriginalFileName, MediaStatus Status, string? ImagePath,
       int UploadedChunks, int TotalChunks, DateTime CreatedAt,
       List<MediaVariantResponse>? Variants)

// MediaVariantResponse
record(string VariantType, int Width, int Height, long FileSize)

// MediaUrlResponse
record(string Url)
```

### Validation Rules

- `FileName`: not empty, max 255 chars
- `ContentType`: must start with `"image/"`
- `TotalFileSize`: 1 byte to 100 MB
- `ChunkIndex`: >= 0
- `ChunkLength`: > 0 and <= 5 MB + 1 KB

## Chunked Upload Protocol

### Step 1: Initiate (`POST /api/media/chunk/init`)

- Client sends `FileName`, `ContentType`, `TotalFileSize`
- Server creates `MediaMetadata` record with `Status = Uploading`
- Calculates `TotalChunks = ceil(TotalFileSize / 5MB)`
- Returns `MediaId`, `TotalChunks`, `ChunkSize` (always 5,242,880 bytes)

### Step 2: Upload Chunks (`POST /api/media/chunk/{mediaId}/{chunkIndex}`)

- Client sends each chunk as multipart form data
- Server validates: media exists, status is `Uploading`, chunk index in range
- Chunk uploaded to MinIO at key `chunk/{mediaId}/{chunkIndex}`
- `MediaChunk` record created, `UploadedChunks` counter incremented
- Client can retry individual chunks (idempotent by `MediaId + ChunkIndex` unique index)

### Step 3: Complete (`POST /api/media/chunk/{mediaId}/complete`)

- Server verifies `UploadedChunks == TotalChunks`
- Status transitions to `Pending`
- `ProcessImageJob` enqueued via Hangfire

## Image Processing (SkiaSharp)

**Processor:** `SkiaSharpProcessor.ProcessAsync()`

**Variants generated (all WebP, quality 80):**

| Variant | Max Size | Filter Quality |
|---------|----------|----------------|
| `original` | Full resolution | -- |
| `display` | 800x800 | High |
| `thumbnail` | 200x200 | High |

**Resize algorithm:** `ratio = min(maxWidth/origWidth, maxHeight/origHeight)`. If ratio >= 1, no resize. Otherwise both dimensions scaled proportionally.

## Database Schema

**Database:** PostgreSQL
**Schema:** `media` (separate from main Dukaan tables)

### Tables

**`media.MediaMetadata`**

| Column | Type | Notes |
|--------|------|-------|
| Id | uuid | PK |
| TenantId | uuid | Indexed, global query filter |
| OriginalFileName | varchar(255) | Original upload filename |
| ContentType | varchar(100) | MIME type (e.g., `image/jpeg`) |
| StagingKey | varchar(500) | Deprecated (legacy field) |
| ImagePath | text | Base storage path after processing (e.g., `media/{tenantId}/{year}/{month}/{mediaId}`) |
| Status | integer | Enum: 0=Uploading, 1=Pending, 2=Completed, 3=Failed |
| TotalChunks | integer | Expected total chunks |
| ChunkSize | bigint | Bytes per chunk (always 5 MB) |
| TotalFileSize | bigint | Total file size in bytes |
| UploadedChunks | integer | Counter of uploaded chunks |
| CreatedAt | timestamptz | Record creation time |
| UpdatedAt | timestamptz | Last update time |
| IsActive | boolean | Soft-delete flag (default true) |

**`media.MediaChunk`**

| Column | Type | Notes |
|--------|------|-------|
| Id | uuid | PK |
| MediaId | uuid | FK → MediaMetadata.Id, CASCADE |
| ChunkIndex | integer | 0-based chunk index |
| StorageKey | varchar(500) | MinIO key: `chunk/{mediaId}/{chunkIndex}` |
| ChunkSize | bigint | Chunk size in bytes |
| UploadedAt | timestamptz | Upload timestamp |

Unique composite index: `(MediaId, ChunkIndex)`

**`media.MediaVariant`**

| Column | Type | Notes |
|--------|------|-------|
| Id | uuid | PK |
| MediaId | uuid | FK → MediaMetadata.Id, CASCADE |
| VariantType | varchar(50) | `"original"`, `"display"`, or `"thumbnail"` |
| StorageKey | varchar(500) | MinIO key: `media/{tenantId}/{year}/{month}/{mediaId}/{variantType}.webp` |
| Width | integer | Pixel width |
| Height | integer | Pixel height |
| FileSize | bigint | File size in bytes |

## MinIO Storage

**Bucket:** `dukaan-media` (single bucket, auto-created if missing)
**Bucket policy:** Public read (set on startup via `DatabaseMigrationHostedService`)

### Folder Structure

```
dukaan-media/
  chunk/
    {mediaId}/
      0              <-- temporary chunk (deleted after processing)
      1
      2
      ...
  media/
    {tenantId}/
      {year}/
        {month}/
          {mediaId}/
            original.webp    <-- permanent variant
            display.webp     <-- permanent variant
            thumbnail.webp   <-- permanent variant
```

**Temporary chunks** (`chunk/{mediaId}/`) are deleted after successful processing.
**Permanent variants** (`media/{tenantId}/{year}/{month}/{mediaId}/`) persist indefinitely.

**Presigned URLs:** Generated with 1-hour expiry. `ExternalEndpoint` config replaces internal Docker hostname with externally accessible address.

## Background Jobs (Hangfire)

**Storage:** PostgreSQL, schema `hangfire_media`
**Worker count:** 2
**Queue:** `media`

### ProcessImageJob

- **Trigger:** Enqueued by `CompleteUploadHandler` via `IJobDispatcher.EnqueueProcessImage(mediaId, tenantId)`
- **Retry:** 3 attempts (delays: 10s, 30s, 60s)
- **Flow:**
  1. Sets tenant context via `tenantProvider.SetTenantId(tenantId)`
  2. Validates media exists, status is `Pending`, all chunks uploaded
  3. Downloads all chunks from MinIO, combines into single stream
  4. Runs `SkiaSharpProcessor.ProcessAsync()` to generate 3 WebP variants
  5. Uploads variants to `media/{tenantId}/{year}/{month}/{mediaId}/{variantType}.webp`
  6. Creates `MediaVariant` records in DB
  7. Sets `media.ImagePath = basePath`
  8. Deletes chunk files from MinIO
  9. Sets status to `Completed`. On exception, sets status to `Failed` and rethrows

### CleanupStagingJob

- **Trigger:** Recurring, `Cron.Daily`
- **Retry:** 1 attempt
- **Flow:**
  1. Finds `MediaMetadata` records with `Status == Uploading` and `CreatedAt < 24 hours ago`
  2. Deletes associated chunks from MinIO
  3. Sets `IsActive = false`

## Inter-Service Communication

### Dukaan (main) → Dukaan.Media (HTTP polling)

**Direction:** One-way, pull-based

**Flow:**
1. Product created/updated with `PendingMediaId` in main Dukaan service
2. `MediaResolutionJob` (Hangfire, every 30 seconds) polls `GET /api/media/{mediaId}`
3. Sends `X-Tenant-Id` header for tenant resolution
4. If response status == 2 (Completed) and `ImagePath` present: sets `product.ImageUrl`, clears `PendingMediaId`
5. If response status == 3 (Failed): clears `PendingMediaId`

**Dukaan.Media does NOT call back to Dukaan.** Communication is entirely pull-based (polling).

## Authentication

**Scheme:** JWT Bearer (HMAC-SHA256)
**Shared secret:** Same `Jwt:Key` as Dukaan main service

**Token validation:**
- `ValidateIssuer = false`
- `ValidateAudience = false`
- `ValidateLifetime = true`
- `ValidateIssuerSigningKey = true`

**Tenant resolution:**
1. JWT `tenant_id` claim (if authenticated)
2. `X-Tenant-Id` header (fallback for unauthenticated scenarios)

## Configuration

```json
{
  "MinIO": {
    "Endpoint": "minio:9000",
    "ExternalEndpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "dukaan-media"
  },
  "Jwt": {
    "Key": "...",
    "ExpireInMinutes": 200
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=dukaan;Username=postgres;Password=password"
  }
}
```

## Dependencies

| Dependency | Purpose | Config Key |
|-----------|---------|-----------|
| PostgreSQL | Metadata database + Hangfire storage | `ConnectionStrings:DefaultConnection` |
| MinIO | Object storage for chunks and processed images | `MinIO:*` |
| Dukaan (main) | Polls for media status (via HTTP) | -- |

## Dockerfile Notes

- Multi-stage build: `base` → `build` → `publish` → `final`
- Base image: `aspnet:10.0`
- Extra package: `libfontconfig1` (required by SkiaSharp on Linux)
- Exposes port 8080

## Error Handling

- **ErrorOr** library (Result pattern)
- `BaseApiController.ToActionResult()` maps errors to HTTP status:
  - NotFound → 404, Validation → 400, Conflict → 409, Unauthorized → 401, Forbidden → 403
- Validation pipeline: `ValidationBehavior<TRequest, TResponse>` runs FluentValidation validators before handler

## CORS

Allows any origin, method, and header (wide-open for development).
