export interface StorefrontProduct {
  id: string;
  name: string;
  description: string | null;
  price: number;
  imageUrl: string | null;
  stockQuantity: number;
  isActive: boolean;
  categoryIds: string[];
}

export interface StorefrontCategory {
  id: string;
  name: string;
  description: string | null;
  parentCategoryId: string | null;
  subCategories: StorefrontCategory[];
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
