import { http } from "@/lib/http";
import type { StorefrontProduct, StorefrontCategory, PagedResponse } from "./types";

const tenantHeaders = (slug: string) => ({ "x-tenant-slug": slug });

export const storefrontApi = {
  getProducts: (slug: string, pageNumber: number, pageSize = 20) =>
    http<PagedResponse<StorefrontProduct>>(
      `/api/storefront/products?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { headers: tenantHeaders(slug) }
    ),

  getProduct: (slug: string, id: string) =>
    http<StorefrontProduct>(`/api/storefront/products/${id}`, {
      headers: tenantHeaders(slug),
    }),

  getCategories: (slug: string) =>
    http<PagedResponse<StorefrontCategory>>(
      `/api/storefront/categories?pageNumber=1&pageSize=50`,
      { headers: tenantHeaders(slug) }
    ),

  getProductsByCategory: (slug: string, categoryId: string, pageNumber: number, pageSize = 20) =>
    http<PagedResponse<StorefrontProduct>>(
      `/api/storefront/categories/${categoryId}/products?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { headers: tenantHeaders(slug) }
    ),
};
