"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

interface Props {
  slug: string;
  totalAmount: number;
  itemCount: number;
}

export function CartSummary({ slug, totalAmount, itemCount }: Props) {
  return (
    <div className="rounded-2xl border border-zinc-200 bg-white p-6 flex flex-col gap-4 shadow-sm">
      <h2 className="text-base font-semibold">Order Summary</h2>
      <Separator />
      <div className="flex justify-between text-sm text-zinc-600">
        <span>Items ({itemCount})</span>
        <span>{fmt.format(totalAmount)}</span>
      </div>
      <Separator />
      <div className="flex justify-between font-semibold text-zinc-900">
        <span>Total</span>
        <span>{fmt.format(totalAmount)}</span>
      </div>
      <Button disabled className="w-full">Proceed to Checkout</Button>
      <Link
        href={`/store/${slug}`}
        className="inline-flex items-center justify-center w-full h-8 rounded-lg border border-border bg-background hover:bg-muted text-sm font-medium transition-colors px-2.5"
      >
        Continue Shopping
      </Link>
    </div>
  );
}
