import { useQuery } from "@tanstack/react-query";
import { storefrontApi } from "./api";

export function useStorefrontProducts(slug: string, pageNumber: number) {
  return useQuery({
    queryKey: ["storefront", slug, "products", pageNumber],
    queryFn: () => storefrontApi.getProducts(slug, pageNumber),
  });
}

export function useStorefrontProduct(slug: string, id: string) {
  return useQuery({
    queryKey: ["storefront", slug, "products", id],
    queryFn: () => storefrontApi.getProduct(slug, id),
  });
}

export function useStorefrontCategories(slug: string) {
  return useQuery({
    queryKey: ["storefront", slug, "categories"],
    queryFn: () => storefrontApi.getCategories(slug),
  });
}

export function useStorefrontProductsByCategory(
  slug: string,
  categoryId: string,
  pageNumber: number
) {
  return useQuery({
    queryKey: ["storefront", slug, "categories", categoryId, "products", pageNumber],
    queryFn: () => storefrontApi.getProductsByCategory(slug, categoryId, pageNumber),
    enabled: !!categoryId,
  });
}
