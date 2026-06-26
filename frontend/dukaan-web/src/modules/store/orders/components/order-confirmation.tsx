"use client";

import Link from "next/link";
import { CheckCircle2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import type { OrderDto } from "../types";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

interface Props {
  order: OrderDto;
  slug: string;
}

export function OrderConfirmation({ order, slug }: Props) {
  return (
    <div className="flex flex-col gap-4 px-4 py-6">
      <div className="flex flex-col items-center text-center">
        <CheckCircle2 className="h-12 w-12 text-green-500 mb-3" />
        <h2 className="text-lg font-semibold">Order Placed!</h2>
        <p className="text-sm text-zinc-500 mt-1">
          Your order <span className="font-mono font-medium">{order.orderNumber}</span> has been placed successfully.
        </p>
      </div>

      <Separator />

      <div>
        <h3 className="text-sm font-semibold mb-2">Order Summary</h3>
        <div className="flex flex-col gap-2">
          {order.items.map((item) => (
            <div key={item.productId} className="flex justify-between text-sm">
              <span className="text-zinc-600">
                {item.productName} &times; {item.quantity}
              </span>
              <span>{fmt.format(item.subtotal)}</span>
            </div>
          ))}
        </div>
      </div>

      <Separator />

      <div className="flex justify-between font-semibold">
        <span>Total</span>
        <span>{fmt.format(order.total)}</span>
      </div>

      <div className="flex flex-col gap-2 mt-4">
        <Link href={`/store/${slug}/orders/${order.id}`}>
          <Button className="w-full">View Order Details</Button>
        </Link>
        <Link href={`/store/${slug}`}>
          <Button variant="outline" className="w-full">
            Continue Shopping
          </Button>
        </Link>
      </div>
    </div>
  );
}
