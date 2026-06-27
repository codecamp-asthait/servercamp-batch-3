export interface Product {
  id: string;
  name: string;
  description: string | null;
  price: number;
  imageUrl: string | null;
  stockQuantity: number;
  isActive: boolean;
  categoryIds: string[];
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface InitiateUploadResponse {
  mediaId: string;
  totalChunks: number;
  chunkSize: number;
}

export interface CompleteUploadResponse {
  mediaId: string;
  status: string;
}

export interface CreateProductRequest {
  name: string;
  description: string | null;
  price: number;
  pendingMediaId: string | null;
  stockQuantity: number;
  categoryIds: string[];
}

export interface UpdateProductRequest {
  name: string;
  description: string | null;
  price: number;
  pendingMediaId: string | null;
  stockQuantity: number;
  isActive: boolean;
}
