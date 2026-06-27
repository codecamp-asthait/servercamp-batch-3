"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { ShoppingCart } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { localStorageService } from "@/lib/local-storage.service";
import { getMediaUrl } from "@/lib/utils";
import { HttpError } from "@/lib/http";
import { useAddCartItem } from "@/modules/store/cart/hooks";
import type { StorefrontProduct } from "../types";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

export function ProductCard({ product }: { product: StorefrontProduct }) {
  const { slug } = useParams<{ slug: string }>();
  const router = useRouter();
  const token = localStorageService.getCustomerToken(slug);
  const { mutate: addItem, isPending } = useAddCartItem(slug, token ?? "");

  const handleAddToCart = (e: React.MouseEvent) => {
    e.preventDefault();
    if (!token) {
      router.push(`/store/${slug}/login?redirect=/store/${slug}`);
      return;
    }
    addItem(
      { productId: product.id, quantity: 1 },
      {
        onSuccess: () => toast.success("Added to cart"),
        onError: (err) => {
          if (err instanceof HttpError && err.status === 400) {
            toast.error("This product is out of stock");
          } else {
            toast.error("Something went wrong, please try again");
          }
        },
      }
    );
  };

  return (
    <Link
      href={`/store/${slug}/products/${product.id}`}
      className="flex flex-col rounded-xl border border-zinc-200 bg-white overflow-hidden hover:shadow-md transition-shadow"
    >
      <div className="aspect-[4/3] w-full overflow-hidden bg-zinc-100">
        {getMediaUrl(product.imageUrl) ? (
          <img src={getMediaUrl(product.imageUrl, "thumbnail")!} alt={product.name} className="h-full w-full object-cover" />
        ) : null}
      </div>
      <div className="flex flex-col gap-1 p-3">
        <p className="text-sm font-medium text-zinc-800 line-clamp-2">{product.name}</p>
        <div className="flex items-center justify-between gap-2 mt-1">
          <div>
            <p className="text-sm font-semibold text-zinc-900">{fmt.format(product.price)}</p>
            {product.stockQuantity === 0 && (
              <span className="text-xs text-zinc-400">Out of stock</span>
            )}
          </div>
          <Button
            variant="secondary"
            size="icon-sm"
            disabled={product.stockQuantity === 0 || isPending}
            onClick={handleAddToCart}
          >
            <ShoppingCart size={15} />
          </Button>
        </div>
      </div>
    </Link>
  );
}
