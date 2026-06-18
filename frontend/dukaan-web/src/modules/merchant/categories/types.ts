export interface Category {
  id: string;
  name: string;
  description: string | null;
  parentCategoryId: string | null;
  subCategories: Category[];
}

export interface CategoryDropdownItem {
  id: string;
  name: string;
  description: string | null;
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
