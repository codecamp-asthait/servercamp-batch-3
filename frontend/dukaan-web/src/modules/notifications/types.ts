export interface NotificationDto {
  id: string;
  eventType: string;
  orderId?: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface NotificationListDto {
  items: NotificationDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface UnreadCountDto {
  count: number;
}
