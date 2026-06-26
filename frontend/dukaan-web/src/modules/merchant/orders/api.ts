import { http } from "@/lib/http";
import { localStorageService } from "@/lib/local-storage.service";
import type { MerchantOrderSummary, MerchantOrder, PagedResponse, UpdateOrderStatusRequest } from "./types";

const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorageService.getToken()}`,
});

export const ordersApi = {
  getAll: (pageNumber: number, pageSize: number) =>
    http<PagedResponse<MerchantOrderSummary>>(
      `/api/merchant/orders?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { headers: authHeaders() }
    ),

  getById: (id: string) =>
    http<MerchantOrder>(`/api/merchant/orders/${id}`, {
      headers: authHeaders(),
    }),

  updateStatus: (id: string, data: UpdateOrderStatusRequest) =>
    http<void>(`/api/merchant/orders/${id}/status`, {
      method: "PUT",
      headers: authHeaders(),
      body: JSON.stringify(data),
    }),
};
