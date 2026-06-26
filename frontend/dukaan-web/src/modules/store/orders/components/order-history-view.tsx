"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { useOrders } from "../hooks";
import { localStorageService } from "@/lib/local-storage.service";
import type { OrderSummaryDto } from "../types";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

interface Props {
  slug: string;
}

export function OrderHistoryView({ slug }: Props) {
  const token = localStorageService.getCustomerToken(slug);
  const router = useRouter();
  const { data: orders, isLoading, isError, refetch } = useOrders(slug, token ?? "", 1);

  if (!token) {
    router.push(`/store/${slug}/login?redirect=/store/${slug}/orders`);
    return null;
  }

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold mb-6">Order History</h1>
        <div className="flex flex-col gap-4">
          {[...Array(3)].map((_, i) => (
            <Skeleton key={i} className="h-20 w-full" />
          ))}
        </div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold mb-6">Order History</h1>
        <div className="flex flex-col items-center gap-4 py-16">
          <p className="text-destructive">Failed to load orders</p>
          <Button variant="outline" onClick={() => refetch()}>
            Retry
          </Button>
        </div>
      </div>
    );
  }

  if (!orders || orders.items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-2xl font-bold mb-6">Order History</h1>
        <div className="flex flex-col items-center justify-center gap-4 py-16">
          <p className="text-zinc-500">No orders yet</p>
          <Link href={`/store/${slug}`}>
            <Button variant="outline">Browse Store</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Order History</h1>
      <div className="flex flex-col gap-4">
        {orders.items.map((order) => (
          <OrderCard key={order.id} order={order} slug={slug} />
        ))}
      </div>
    </div>
  );
}

function OrderCard({ order, slug }: { order: OrderSummaryDto; slug: string }) {
  const statusColor =
    order.status === "Pending"
      ? "bg-yellow-100 text-yellow-800"
      : order.status === "Confirmed"
      ? "bg-green-100 text-green-800"
      : "bg-red-100 text-red-800";

  return (
    <Link href={`/store/${slug}/orders/${order.id}`}>
      <div className="flex items-center justify-between p-4 rounded-lg border border-zinc-200 hover:border-zinc-300 transition-colors">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2">
            <span className="font-mono font-medium">{order.orderNumber}</span>
            <Badge className={statusColor}>{order.status}</Badge>
          </div>
          <p className="text-sm text-zinc-500">
            {new Date(order.createdAt).toLocaleDateString()} · {order.itemCount} item
            {order.itemCount !== 1 ? "s" : ""}
          </p>
        </div>
        <div className="text-right">
          <p className="font-semibold">{fmt.format(order.total)}</p>
        </div>
      </div>
    </Link>
  );
}
