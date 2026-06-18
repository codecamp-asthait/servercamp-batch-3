import { http } from "@/lib/http";
import type { CustomerLoginRequest, CustomerRegisterRequest, CustomerAuthResponse } from "./types";

const tenantHeaders = (slug: string) => ({
  "Content-Type": "application/json",
  "x-tenant-slug": slug,
});

export const customerAuthApi = {
  login: (slug: string, data: CustomerLoginRequest) =>
    http<CustomerAuthResponse>("/api/auth/customer/login", {
      method: "POST",
      headers: tenantHeaders(slug),
      body: JSON.stringify(data),
    }),

  register: (slug: string, data: CustomerRegisterRequest) =>
    http<{ customerId: string }>("/api/customers/register", {
      method: "POST",
      headers: tenantHeaders(slug),
      body: JSON.stringify(data),
    }),
};
