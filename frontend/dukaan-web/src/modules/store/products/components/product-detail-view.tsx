"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { Heart, Minus, Plus, ShoppingCart } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { localStorageService } from "@/lib/local-storage.service";
import { getMediaUrl } from "@/lib/utils";
import { HttpError } from "@/lib/http";
import { useAddCartItem } from "@/modules/store/cart/hooks";
import { useStorefrontProduct } from "../hooks";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

export function ProductDetailView() {
  const { slug, id } = useParams<{ slug: string; id: string }>();
  const router = useRouter();
  const token = localStorageService.getCustomerToken(slug);
  const { data: product, isLoading, error } = useStorefrontProduct(slug, id);
  const { mutate: addItem, isPending } = useAddCartItem(slug, token ?? "");
  const [quantity, setQuantity] = useState(1);

  const storeName = slug
    .split("-")
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ");

  const handleAddToCart = () => {
    if (!token) {
      router.push(`/store/${slug}/login?redirect=/store/${slug}/products/${id}`);
      return;
    }
    addItem(
      { productId: id, quantity },
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

  if (isLoading) {
    return (
      <div className="flex-1 overflow-y-auto p-8 animate-pulse">
        <div className="mx-auto max-w-6xl flex flex-col gap-8">
          <div className="h-4 w-48 bg-zinc-100 rounded" />
          <div className="flex flex-col md:flex-row gap-8 lg:gap-12">
            <div className="w-full md:w-[480px] aspect-[4/3] rounded-2xl bg-zinc-100 shrink-0" />
            <div className="flex flex-col gap-5 flex-1 pt-2">
              <div className="h-7 w-3/4 bg-zinc-100 rounded" />
              <div className="h-8 w-28 bg-zinc-100 rounded" />
              <div className="h-6 w-20 bg-zinc-100 rounded-full" />
              <div className="h-px w-full bg-zinc-100" />
              <div className="h-14 w-24 bg-zinc-100 rounded-xl" />
              <div className="h-14 flex-1 bg-zinc-100 rounded-xl" />
              <div className="h-14 w-14 bg-zinc-100 rounded-xl" />
              <div className="h-14 w-full bg-zinc-100 rounded-xl mt-1" />
              <div className="h-px w-full bg-zinc-100 mt-4" />
              <div className="space-y-2 mt-4">
                <div className="h-4 w-full bg-zinc-100 rounded" />
                <div className="h-4 w-5/6 bg-zinc-100 rounded" />
                <div className="h-4 w-4/6 bg-zinc-100 rounded" />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error instanceof HttpError && (error.status === 404 || error.status === 400)) {
    return <p className="p-8 text-sm text-zinc-500">Product not found.</p>;
  }

  if (!product) return null;

  const outOfStock = product.stockQuantity === 0;

  return (
    <div className="flex-1 overflow-y-auto p-8">
      <div className="mx-auto max-w-6xl flex flex-col gap-8">
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem>
              <Link href={`/store/${slug}`} className="transition-colors hover:text-foreground">Home</Link>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <Link href={`/store/${slug}`} className="transition-colors hover:text-foreground">{storeName}</Link>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbPage>{product.name}</BreadcrumbPage>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>

        <div className="flex flex-col md:flex-row gap-8 lg:gap-12">
          <div className="w-full md:w-[480px] shrink-0">
            {getMediaUrl(product.imageUrl) ? (
              <div className="aspect-[4/3] w-full overflow-hidden rounded-2xl bg-zinc-50 border border-zinc-200">
                <img
                  src={getMediaUrl(product.imageUrl)!}
                  alt={product.name}
                  className="h-full w-full object-cover object-center transition-transform duration-500 hover:scale-105"
                />
              </div>
            ) : (
              <div className="aspect-[4/3] w-full rounded-2xl bg-zinc-100 shrink-0" />
            )}
          </div>

          <div className="flex flex-col gap-6 flex-1 pt-2">
            <div className="flex flex-col gap-3">
              <h1 className="text-3xl font-serif font-bold text-zinc-900 leading-tight">
                {product.name}
              </h1>
              <p className="text-2xl font-semibold text-zinc-900">
                {fmt.format(product.price)}
              </p>

              {outOfStock ? (
                <span className="inline-flex items-center gap-1.5 rounded-full bg-zinc-100 px-3 py-1 text-sm font-medium text-zinc-500 w-fit">
                  <span className="h-1.5 w-1.5 rounded-full bg-zinc-400" />
                  Out of stock
                </span>
              ) : (
                <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-3 py-1 text-sm font-medium text-emerald-700 w-fit">
                  <span className="h-1.5 w-1.5 rounded-full bg-emerald-500" />
                  In stock
                </span>
              )}
            </div>

            <hr className="border-zinc-200" />

            <div className="flex flex-col gap-4">
              <div className="flex gap-4">
                <div className="flex items-center rounded-xl border border-zinc-200 bg-white h-14">
                  <button
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    className="px-4 text-zinc-500 hover:text-zinc-900 transition-colors h-full flex items-center disabled:opacity-40"
                    disabled={outOfStock}
                  >
                    <Minus className="h-4 w-4" />
                  </button>
                  <span className="w-8 text-center font-medium text-zinc-900">
                    {quantity}
                  </span>
                  <button
                    onClick={() => setQuantity(quantity + 1)}
                    className="px-4 text-zinc-500 hover:text-zinc-900 transition-colors h-full flex items-center disabled:opacity-40"
                    disabled={outOfStock}
                  >
                    <Plus className="h-4 w-4" />
                  </button>
                </div>
                <Button
                  className="flex-1 h-14 rounded-xl font-medium flex items-center justify-center gap-2 shadow-sm"
                  disabled={outOfStock || isPending}
                  onClick={handleAddToCart}
                >
                  {isPending ? (
                    "Adding…"
                  ) : (
                    <>
                      <ShoppingCart className="h-5 w-5" />
                      Add to Cart
                    </>
                  )}
                </Button>
                <button
                  className="flex h-14 w-14 items-center justify-center rounded-xl border border-zinc-200 bg-white text-zinc-600 hover:border-zinc-300 hover:text-rose-500 transition-all"
                  aria-label="Add to wishlist"
                >
                  <Heart className="h-5 w-5" />
                </button>
              </div>
            </div>

            {product.description && (
              <>
                <hr className="border-zinc-200" />
                <div>
                  <h2 className="text-lg font-semibold text-zinc-900 mb-3">
                    Details
                  </h2>
                  <div
                    className="text-sm text-zinc-600 leading-relaxed prose prose-sm max-w-none"
                    dangerouslySetInnerHTML={{ __html: product.description }}
                  />
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
