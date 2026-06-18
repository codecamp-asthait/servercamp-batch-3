using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dukaan.Media.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMediaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "media");

            migrationBuilder.CreateTable(
                name: "MediaMetadata",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StagingKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalChunks = table.Column<int>(type: "integer", nullable: false),
                    ChunkSize = table.Column<long>(type: "bigint", nullable: false),
                    TotalFileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedChunks = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaMetadata", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaChunk",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ChunkSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaChunk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaChunk_MediaMetadata_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "media",
                        principalTable: "MediaMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaVariant",
                schema: "media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    VariantType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaVariant_MediaMetadata_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "media",
                        principalTable: "MediaMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaChunk_MediaId_ChunkIndex",
                schema: "media",
                table: "MediaChunk",
                columns: new[] { "MediaId", "ChunkIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaMetadata_TenantId",
                schema: "media",
                table: "MediaMetadata",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaVariant_MediaId",
                schema: "media",
                table: "MediaVariant",
                column: "MediaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaChunk",
                schema: "media");

            migrationBuilder.DropTable(
                name: "MediaVariant",
                schema: "media");

            migrationBuilder.DropTable(
                name: "MediaMetadata",
                schema: "media");
        }
    }
}
