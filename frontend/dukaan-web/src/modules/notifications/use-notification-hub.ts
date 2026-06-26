"use client";

import { useEffect, useRef, useState, useCallback } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { notificationApi } from "./api";
import type { NotificationDto } from "./types";

const HUB_URL = `${process.env.NEXT_PUBLIC_NOTIFICATION_API_URL}/hubs/notifications`;

interface UseNotificationHubOptions {
  token: string | null;
  enabled: boolean;
}

export function useNotificationHub({ token, enabled }: UseNotificationHubOptions) {
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const connectionRef = useRef<ReturnType<typeof HubConnectionBuilder.prototype.build> | null>(null);

  const markAsRead = useCallback(async (id: string) => {
    if (!token) return;
    try {
      await notificationApi.markAsRead(token, id);
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch {
      // ignore
    }
  }, [token]);

  useEffect(() => {
    if (!token || !enabled) return;

    const connection = new HubConnectionBuilder()
      .withUrl(HUB_URL, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection.on("Notification", (notification: NotificationDto) => {
      setNotifications((prev) => [notification, ...prev]);
      setUnreadCount((prev) => prev + 1);
    });

    connection.start().catch(() => {
      // connection failed — will auto-retry
    });

    return () => {
      connection.stop();
      connectionRef.current = null;
    };
  }, [token, enabled]);

  useEffect(() => {
    if (!token || !enabled) return;
    notificationApi.unreadCount(token).then((res) => {
      setUnreadCount(res.count);
    }).catch(() => {});
    notificationApi.list(token, 1, 10).then((res) => {
      setNotifications(res.items);
    }).catch(() => {});
  }, [token, enabled]);

  return { notifications, unreadCount, markAsRead };
}
