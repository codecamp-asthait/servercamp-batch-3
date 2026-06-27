"use client";

import { OrderHistoryView } from "@/modules/store/orders/components/order-history-view";

interface OrdersTabProps {
  slug: string;
}

export function OrdersTab({ slug }: OrdersTabProps) {
  return <OrderHistoryView slug={slug} />;
}
