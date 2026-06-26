import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "./api";
import type { UpdateOrderStatusRequest } from "./types";

export function useOrders(pageNumber: number, pageSize = 10) {
  return useQuery({
    queryKey: ["merchant-orders", pageNumber, pageSize],
    queryFn: () => ordersApi.getAll(pageNumber, pageSize),
  });
}

export function useOrder(id: string) {
  return useQuery({
    queryKey: ["merchant-order", id],
    queryFn: () => ordersApi.getById(id),
    enabled: !!id,
  });
}

export function useUpdateOrderStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateOrderStatusRequest }) =>
      ordersApi.updateStatus(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["merchant-orders"] });
      queryClient.invalidateQueries({ queryKey: ["merchant-order"] });
    },
  });
}
