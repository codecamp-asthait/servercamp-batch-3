import { http } from "@/lib/http";
import { localStorageService } from "@/lib/local-storage.service";
import type { Category, CategoryDropdownItem, PagedResponse } from "./types";

const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorageService.getToken()}`,
});

export const categoriesApi = {
  getAll: () =>
    http<PagedResponse<Category>>(`/api/categories?pageNumber=1&pageSize=50`, {
      headers: authHeaders(),
    }),

  getDropdown: () =>
    http<CategoryDropdownItem[]>(`/api/categories/dropdown`, {
      headers: authHeaders(),
    }),
};
