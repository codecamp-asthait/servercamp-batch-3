import { http } from "@/lib/http";
import type {
  OrderDto,
  AddressDto,
  OrderSummaryDto,
  PlaceOrderRequest,
  CreateAddressData,
  UpdateAddressData,
  PagedResponse,
} from "./types";

const headers = (slug: string, token: string) => ({
  "Content-Type": "application/json",
  "x-tenant-slug": slug,
  Authorization: `Bearer ${token}`,
});

export const ordersApi = {
  placeOrder: (slug: string, token: string, body: PlaceOrderRequest) =>
    http<OrderDto>("/api/orders", {
      method: "POST",
      headers: headers(slug, token),
      body: JSON.stringify(body),
    }),

  getAddresses: (slug: string, token: string) =>
    http<AddressDto[]>("/api/addresses", {
      headers: headers(slug, token),
    }),

  createAddress: (slug: string, token: string, body: CreateAddressData) =>
    http<AddressDto>("/api/addresses", {
      method: "POST",
      headers: headers(slug, token),
      body: JSON.stringify(body),
    }),

  updateAddress: (slug: string, token: string, id: string, body: UpdateAddressData) =>
    http<AddressDto>(`/api/addresses/${id}`, {
      method: "PUT",
      headers: headers(slug, token),
      body: JSON.stringify(body),
    }),

  deleteAddress: (slug: string, token: string, id: string) =>
    http<void>(`/api/addresses/${id}`, {
      method: "DELETE",
      headers: headers(slug, token),
    }),

  setDefaultAddress: (slug: string, token: string, id: string) =>
    http<AddressDto>(`/api/addresses/${id}/set-default`, {
      method: "PATCH",
      headers: headers(slug, token),
    }),

  getOrders: (slug: string, token: string, pageNumber: number, pageSize = 10) =>
    http<PagedResponse<OrderSummaryDto>>(
      `/api/orders?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      {
        headers: headers(slug, token),
      }
    ),

  getOrder: (slug: string, token: string, orderId: string) =>
    http<OrderDto>(`/api/orders/${orderId}`, {
      headers: headers(slug, token),
    }),
};
