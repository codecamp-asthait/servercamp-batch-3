import { http } from "@/lib/http";
import type { CustomerProfileDto, UpdateCustomerProfileData } from "./types";

function headers(slug: string, token: string) {
  return {
    "Content-Type": "application/json",
    "x-tenant-slug": slug,
    Authorization: `Bearer ${token}`,
  };
}

export const profileApi = {
  get: (slug: string, token: string) =>
    http<CustomerProfileDto>("/api/customers/me/profile", {
      headers: headers(slug, token),
    }),

  update: (slug: string, token: string, data: UpdateCustomerProfileData) =>
    http<CustomerProfileDto>("/api/customers/me/profile", {
      method: "PUT",
      headers: headers(slug, token),
      body: JSON.stringify(data),
    }),
};
