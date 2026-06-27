import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productsApi } from "./api";
import type { CreateProductRequest, UpdateProductRequest } from "./types";

export function useProducts(pageNumber: number, pageSize = 10) {
  return useQuery({
    queryKey: ["products", pageNumber, pageSize],
    queryFn: () => productsApi.getAll(pageNumber, pageSize),
  });
}

export function useProductById(id: string | null) {
  return useQuery({
    queryKey: ["product", id],
    queryFn: () => productsApi.getById(id!),
    enabled: !!id,
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

export function useUpdateProduct(onSuccess: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProductRequest }) =>
      productsApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      onSuccess();
    },
  });
}

export function useDeleteProduct(onSuccess: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => productsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      onSuccess();
    },
  });
}
