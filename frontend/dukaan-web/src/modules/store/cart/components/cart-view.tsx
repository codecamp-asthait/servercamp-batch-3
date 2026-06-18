"use client";

import { useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { ShoppingCart } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { localStorageService } from "@/lib/local-storage.service";
import { useCart, useUpdateCartItem, useRemoveCartItem } from "../hooks";
import { CartItemRow } from "./cart-item-row";
import { CartSummary } from "./cart-summary";

function CartSkeleton() {
  return (
    <div className="flex flex-col">
      {[...Array(3)].map((_, i) => (
        <div key={i} className="flex gap-4 py-5 border-b border-zinc-100">
          <Skeleton className="h-20 w-20 rounded-xl" />
          <div className="flex-1 flex flex-col gap-2 pt-1">
            <Skeleton className="h-4 w-48" />
            <Skeleton className="h-3 w-24" />
            <Skeleton className="h-7 w-28 mt-2" />
          </div>
        </div>
      ))}
    </div>
  );
}

export function CartView() {
  const { slug } = useParams<{ slug: string }>();
  const router = useRouter();
  const token = localStorageService.getCustomerToken(slug);

  useEffect(() => {
    if (!token) router.replace(`/store/${slug}/login?redirect=/store/${slug}/cart`);
  }, [token, slug, router]);

  const { data: cart, isLoading, isError, refetch } = useCart(slug, token);
  const { mutate: updateItem } = useUpdateCartItem(slug, token ?? "");
  const { mutate: removeItem } = useRemoveCartItem(slug, token ?? "");

  const handleUpdate = (productId: string, quantity: number) =>
    updateItem({ productId, quantity }, {
      onError: () => toast.error("Something went wrong, please try again"),
    });

  const handleRemove = (productId: string) =>
    removeItem(productId, {
      onError: () => toast.error("Something went wrong, please try again"),
    });

  if (!token) return null;

  if (isError) return (
    <main className="flex-1 overflow-y-auto flex flex-col items-center justify-center gap-4 px-4">
      <p className="text-sm text-destructive">Failed to load your cart.</p>
      <Button variant="outline" onClick={() => refetch()}>Retry</Button>
    </main>
  );

  const itemCount = cart?.items.reduce((sum, i) => sum + i.quantity, 0) ?? 0;

  return (
    <main className="flex-1 overflow-y-auto max-w-4xl mx-auto w-full px-4 py-10">
      <h1 className="text-2xl font-bold text-zinc-900 mb-8">Your Cart</h1>

      {isLoading ? (
        <CartSkeleton />
      ) : !cart || cart.items.length === 0 ? (
        <div className="flex flex-col items-center gap-6 py-20 text-center">
          <ShoppingCart size={48} className="text-zinc-300" />
          <div>
            <p className="font-medium text-zinc-700">Your cart is empty</p>
            <p className="text-sm text-zinc-500 mt-1">Add some products to get started.</p>
          </div>
          <Link
            href={`/store/${slug}`}
            className="inline-flex items-center justify-center h-8 rounded-lg bg-primary text-primary-foreground text-sm font-medium px-2.5 transition-colors hover:bg-primary/80"
          >
            Browse Products
          </Link>
        </div>
      ) : (
        <div className="flex flex-col gap-8 lg:flex-row lg:items-start">
          <div className="flex-1 rounded-2xl border border-zinc-200 bg-white px-6 shadow-sm">
            {cart.items.map((item) => (
              <CartItemRow key={item.productId} item={item} onUpdate={handleUpdate} onRemove={handleRemove} />
            ))}
          </div>
          <div className="lg:w-72 lg:sticky lg:top-6">
            <CartSummary slug={slug} totalAmount={cart.total} itemCount={itemCount} />
          </div>
        </div>
      )}
    </main>
  );
}
