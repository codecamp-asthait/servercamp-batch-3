import { http } from "@/lib/http";
import type { NotificationListDto, UnreadCountDto } from "./types";

const NOTIFICATION_BASE_URL = process.env.NEXT_PUBLIC_NOTIFICATION_API_URL;

function authHeaders(token: string) {
  return {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };
}

export const notificationApi = {
  list: (token: string, page = 1, pageSize = 10, unreadOnly = false) => {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    if (unreadOnly) params.set("unreadOnly", "true");
    return http<NotificationListDto>(`/api/notifications?${params}`, {
      headers: authHeaders(token),
      baseUrl: NOTIFICATION_BASE_URL,
    });
  },

  unreadCount: (token: string) =>
    http<UnreadCountDto>("/api/notifications/unread-count", {
      headers: authHeaders(token),
      baseUrl: NOTIFICATION_BASE_URL,
    }),

  markAsRead: (token: string, id: string) =>
    http<void>(`/api/notifications/${id}/read`, {
      method: "POST",
      headers: authHeaders(token),
      baseUrl: NOTIFICATION_BASE_URL,
    }),
};
