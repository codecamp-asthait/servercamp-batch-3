import { http } from "@/lib/http";
import { localStorageService } from "@/lib/local-storage.service";
import type {
  Product,
  PagedResponse,
  CreateProductRequest,
  InitiateUploadResponse,
  CompleteUploadResponse,
} from "./types";

const MEDIA_BASE = process.env.NEXT_PUBLIC_MEDIA_API_URL;

const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorageService.getToken()}`,
});

export const productsApi = {
  getAll: (pageNumber: number, pageSize: number) =>
    http<PagedResponse<Product>>(
      `/api/products?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { headers: authHeaders() }
    ),

  create: (data: CreateProductRequest) =>
    http<Product>("/api/products", {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(data),
    }),
};

const mediaAuthHeaders = () => ({
  Authorization: `Bearer ${localStorageService.getToken()}`,
});

export const mediaApi = {
  initiateUpload: (fileName: string, contentType: string, totalFileSize: number) =>
    http<InitiateUploadResponse>(`/api/media/chunk/init`, {
      method: "POST",
      baseUrl: MEDIA_BASE,
      headers: { ...mediaAuthHeaders(), "Content-Type": "application/json" },
      body: JSON.stringify({ fileName, contentType, totalFileSize }),
    }),

  uploadChunk: (mediaId: string, chunkIndex: number, chunk: Blob) => {
    const form = new FormData();
    form.append("chunk", chunk);
    return http<unknown>(`/api/media/chunk/${mediaId}/${chunkIndex}`, {
      method: "POST",
      baseUrl: MEDIA_BASE,
      headers: mediaAuthHeaders(),
      body: form,
    });
  },

  completeUpload: (mediaId: string) =>
    http<CompleteUploadResponse>(`/api/media/chunk/${mediaId}/complete`, {
      method: "POST",
      baseUrl: MEDIA_BASE,
      headers: { ...mediaAuthHeaders(), "Content-Type": "application/json" },
    }),
};
