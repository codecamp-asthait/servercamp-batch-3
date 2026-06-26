"use client";

import { useState } from "react";
import Link from "next/link";
import { ShoppingCart } from "lucide-react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  SheetFooter,
} from "@/components/ui/sheet";
import { useCart, useUpdateCartItem, useRemoveCartItem } from "../hooks";
import { CartItemRow } from "./cart-item-row";
import { CheckoutView } from "@/modules/store/orders/components/checkout-view";
import { OrderConfirmation } from "@/modules/store/orders/components/order-confirmation";
import { useOrder } from "@/modules/store/orders/hooks";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

function CartSkeleton() {
  return (
    <div className="flex flex-col px-4">
      {[...Array(3)].map((_, i) => (
        <div key={i} className="flex gap-3 py-4 border-b border-zinc-100">
          <Skeleton className="h-16 w-16 rounded-xl flex-shrink-0" />
          <div className="flex-1 flex flex-col gap-2 pt-1">
            <Skeleton className="h-3 w-36" />
            <Skeleton className="h-3 w-20" />
            <Skeleton className="h-6 w-24 mt-1" />
          </div>
        </div>
      ))}
    </div>
  );
}

interface Props {
  slug: string;
  token: string | null;
}

type DrawerView = "cart" | "checkout" | "confirmation";

export function CartDrawer({ slug, token }: Props) {
  const { data: cart, isLoading, isError, refetch } = useCart(slug, token);
  const { mutate: updateItem } = useUpdateCartItem(slug, token ?? "");
  const { mutate: removeItem } = useRemoveCartItem(slug, token ?? "");

  const [view, setView] = useState<DrawerView>("cart");
  const [placedOrderId, setPlacedOrderId] = useState<string | null>(null);
  const { data: placedOrder } = useOrder(slug, token ?? "", placedOrderId ?? "");

  const itemCount = cart?.items.reduce((sum, i) => sum + i.quantity, 0) ?? 0;

  const handleUpdate = (productId: string, quantity: number) =>
    updateItem({ productId, quantity }, {
      onError: () => toast.error("Something went wrong, please try again"),
    });

  const handleRemove = (productId: string) =>
    removeItem(productId, {
      onError: () => toast.error("Something went wrong, please try again"),
    });

  const handleCheckoutSuccess = (orderId: string) => {
    setPlacedOrderId(orderId);
    setView("confirmation");
    refetch();
  };

  const handleBackToCart = () => {
    setView("cart");
    setPlacedOrderId(null);
  };

  return (
    <Sheet>
      <SheetTrigger
        className="relative inline-flex items-center justify-center size-8 rounded-lg hover:bg-muted transition-colors"
        aria-label="Open cart"
      >
        <ShoppingCart size={20} />
        {itemCount > 0 && (
          <Badge className="absolute -top-1 -right-1 h-4 w-4 p-0 flex items-center justify-center text-[10px]">
            {itemCount > 9 ? "9+" : itemCount}
          </Badge>
        )}
      </SheetTrigger>

      <SheetContent className="flex flex-col p-0" side="right">
        <SheetHeader className="px-4 pt-4 pb-3 border-b border-zinc-100">
          <SheetTitle>
            {view === "cart" && `Your Cart ${itemCount > 0 ? `(${itemCount})` : ""}`}
            {view === "checkout" && "Checkout"}
            {view === "confirmation" && "Order Placed!"}
          </SheetTitle>
        </SheetHeader>

        <div className="flex-1 overflow-y-auto">
          {view === "cart" && (
            <>
              {!token ? (
                <div className="flex flex-col items-center gap-4 py-16 px-4 text-center">
                  <ShoppingCart size={40} className="text-zinc-300" />
                  <p className="text-sm text-zinc-500">Sign in to view your cart.</p>
                  <Link
                    href={`/store/${slug}/login`}
                    className="inline-flex items-center justify-center h-8 rounded-lg bg-primary text-primary-foreground text-sm font-medium px-4 transition-colors hover:bg-primary/80"
                  >
                    Sign in
                  </Link>
                </div>
              ) : isLoading ? (
                <CartSkeleton />
              ) : isError ? (
                <div className="flex flex-col items-center gap-3 py-16 px-4 text-center">
                  <p className="text-sm text-destructive">Failed to load cart.</p>
                  <Button variant="outline" size="sm" onClick={() => refetch()}>Retry</Button>
                </div>
              ) : !cart || cart.items.length === 0 ? (
                <div className="flex flex-col items-center justify-center gap-2 h-full text-center">
                  <ShoppingCart size={40} className="text-zinc-300" />
                  <p className="text-sm text-zinc-500">Your cart is empty.</p>
                </div>
              ) : (
                <div className="px-4">
                  {cart.items.map((item) => (
                    <CartItemRow key={item.productId} item={item} onUpdate={handleUpdate} onRemove={handleRemove} />
                  ))}
                </div>
              )}
            </>
          )}

          {view === "checkout" && token && (
            <CheckoutView
              slug={slug}
              token={token}
              onBack={handleBackToCart}
              onSuccess={handleCheckoutSuccess}
            />
          )}

          {view === "confirmation" && placedOrder && (
            <OrderConfirmation order={placedOrder} slug={slug} />
          )}
        </div>

        {view === "cart" && cart && cart.items.length > 0 && (
          <SheetFooter className="flex flex-col gap-3 px-4 pb-4 pt-3 border-t border-zinc-100">
            <div className="flex justify-between text-sm text-zinc-600">
              <span>Items ({itemCount})</span>
              <span>{fmt.format(cart.total)}</span>
            </div>
            <Separator />
            <div className="flex justify-between font-semibold text-zinc-900">
              <span>Total</span>
              <span>{fmt.format(cart.total)}</span>
            </div>
            <Button
              className="w-full"
              onClick={() => setView("checkout")}
              disabled={!token}
            >
              Proceed to Checkout
            </Button>
          </SheetFooter>
        )}
      </SheetContent>
    </Sheet>
  );
}
