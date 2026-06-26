"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { useOrder } from "../hooks";
import { localStorageService } from "@/lib/local-storage.service";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

interface Props {
  slug: string;
  orderId: string;
}

export function OrderDetailView({ slug, orderId }: Props) {
  const token = localStorageService.getCustomerToken(slug);
  const router = useRouter();
  const { data: order, isLoading, isError } = useOrder(slug, token ?? "", orderId);

  if (!token) {
    router.push(`/store/${slug}/login?redirect=/store/${slug}/orders/${orderId}`);
    return null;
  }

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col gap-4">
          <Skeleton className="h-8 w-48" />
          <Skeleton className="h-32 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
      </div>
    );
  }

  if (isError || !order) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col items-center justify-center gap-4 py-16">
          <p className="text-zinc-500">Order not found</p>
          <Link href={`/store/${slug}/orders`}>
            <Button variant="outline">Back to Orders</Button>
          </Link>
        </div>
      </div>
    );
  }

  const statusColor =
    order.status === "Pending"
      ? "bg-yellow-100 text-yellow-800"
      : order.status === "Confirmed"
      ? "bg-green-100 text-green-800"
      : "bg-red-100 text-red-800";

  return (
    <div className="container mx-auto px-4 py-8">
      <Link href={`/store/${slug}/orders`}>
        <Button variant="ghost" className="mb-4">
          &larr; Back to Orders
        </Button>
      </Link>

      <div className="flex items-center gap-3 mb-6">
        <h1 className="text-2xl font-bold">{order.orderNumber}</h1>
        <Badge className={statusColor}>{order.status}</Badge>
      </div>

      <p className="text-sm text-zinc-500 mb-6">
        Placed on {new Date(order.createdAt).toLocaleDateString()}
      </p>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div>
          <h2 className="text-sm font-semibold mb-2">Billing Address</h2>
          <div className="p-4 rounded-lg border border-zinc-200">
            <p className="text-sm">
              {order.billingAddress.street}
              <br />
              {order.billingAddress.city}, {order.billingAddress.district}{" "}
              {order.billingAddress.postalCode}
              <br />
              {order.billingAddress.phone}
            </p>
          </div>
        </div>

        <div>
          <h2 className="text-sm font-semibold mb-2">Delivery Address</h2>
          <div className="p-4 rounded-lg border border-zinc-200">
            <p className="text-sm">
              {order.deliveryAddress.street}
              <br />
              {order.deliveryAddress.city}, {order.deliveryAddress.district}{" "}
              {order.deliveryAddress.postalCode}
              <br />
              {order.deliveryAddress.phone}
            </p>
          </div>
        </div>
      </div>

      <Separator className="my-6" />

      <div>
        <h2 className="text-sm font-semibold mb-4">Order Items</h2>
        <div className="flex flex-col gap-3">
          {order.items.map((item) => (
            <div key={item.productId} className="flex justify-between items-center">
              <div>
                <p className="font-medium">{item.productName}</p>
                <p className="text-sm text-zinc-500">
                  {fmt.format(item.unitPrice)} &times; {item.quantity}
                </p>
              </div>
              <p className="font-semibold">{fmt.format(item.subtotal)}</p>
            </div>
          ))}
        </div>
      </div>

      <Separator className="my-6" />

      <div className="flex justify-between font-semibold text-lg">
        <span>Total</span>
        <span>{fmt.format(order.total)}</span>
      </div>
    </div>
  );
}
