import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "./api";
import type { PlaceOrderRequest } from "./types";

const ordersListKey = (slug: string) => ["storefront", slug, "orders"];
const orderDetailKey = (slug: string, id: string) => ["storefront", slug, "orders", id];
const addressesKey = (slug: string) => ["storefront", slug, "addresses"];

export function usePlaceOrder(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: PlaceOrderRequest) => ordersApi.placeOrder(slug, token, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ordersListKey(slug) });
    },
  });
}

export function useOrders(slug: string, token: string, pageNumber: number) {
  return useQuery({
    queryKey: ordersListKey(slug),
    queryFn: () => ordersApi.getOrders(slug, token, pageNumber),
    enabled: !!token,
  });
}

export function useOrder(slug: string, token: string, orderId: string) {
  return useQuery({
    queryKey: orderDetailKey(slug, orderId),
    queryFn: () => ordersApi.getOrder(slug, token, orderId),
    enabled: !!token && !!orderId,
  });
}

export function useCustomerAddresses(slug: string, token: string) {
  return useQuery({
    queryKey: addressesKey(slug),
    queryFn: () => ordersApi.getAddresses(slug, token),
    enabled: !!token,
  });
}

export function useCreateAddress(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: import("./types").CreateAddressData) => ordersApi.createAddress(slug, token, body),
    onSuccess: () => qc.invalidateQueries({ queryKey: addressesKey(slug) }),
  });
}

export function useUpdateAddress(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: import("./types").UpdateAddressData }) =>
      ordersApi.updateAddress(slug, token, id, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: addressesKey(slug) }),
  });
}

export function useDeleteAddress(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => ordersApi.deleteAddress(slug, token, id),
    onSuccess: () => qc.invalidateQueries({ queryKey: addressesKey(slug) }),
  });
}

export function useSetDefaultAddress(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => ordersApi.setDefaultAddress(slug, token, id),
    onSuccess: () => qc.invalidateQueries({ queryKey: addressesKey(slug) }),
  });
}
