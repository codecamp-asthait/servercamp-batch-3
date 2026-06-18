import { http } from "@/lib/http";
import type { CartDto, AddCartItemRequest, UpdateCartItemRequest } from "./types";

const headers = (slug: string, token: string) => ({
  "Content-Type": "application/json",
  "x-tenant-slug": slug,
  Authorization: `Bearer ${token}`,
});

export const cartApi = {
  getCart: (slug: string, token: string) =>
    http<CartDto>("/api/cart", { headers: headers(slug, token) }),

  addItem: (slug: string, token: string, body: AddCartItemRequest) =>
    http<CartDto>("/api/cart/items", {
      method: "POST",
      headers: headers(slug, token),
      body: JSON.stringify(body),
    }),

  updateItem: (slug: string, token: string, productId: string, body: UpdateCartItemRequest) =>
    http<CartDto>(`/api/cart/items/${productId}`, {
      method: "PUT",
      headers: headers(slug, token),
      body: JSON.stringify(body),
    }),

  removeItem: (slug: string, token: string, productId: string) =>
    http<CartDto>(`/api/cart/items/${productId}`, {
      method: "DELETE",
      headers: headers(slug, token),
    }),

  clearCart: (slug: string, token: string) =>
    http<void>("/api/cart", {
      method: "DELETE",
      headers: headers(slug, token),
    }),
};
