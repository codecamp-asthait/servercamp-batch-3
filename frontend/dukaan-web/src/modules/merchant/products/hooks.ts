import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productsApi } from "./api";
import type { CreateProductRequest } from "./types";

export function useProducts(pageNumber: number, pageSize = 10) {
  return useQuery({
    queryKey: ["products", pageNumber, pageSize],
    queryFn: () => productsApi.getAll(pageNumber, pageSize),
  });
}

export function useCreateProduct(onSuccess: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateProductRequest) => productsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      onSuccess();
    },
  });
}
