"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
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

  const handleAddToCart = () => {
    if (!token) {
      router.push(`/store/${slug}/login?redirect=/store/${slug}/products/${id}`);
      return;
    }
    addItem(
      { productId: id, quantity: 1 },
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
      <div className="flex-1 overflow-y-auto p-8 flex gap-8 animate-pulse">
        <div className="w-96 h-96 rounded-xl bg-zinc-100 shrink-0" />
        <div className="flex flex-col gap-3 flex-1">
          <div className="h-6 w-2/3 bg-zinc-100 rounded" />
          <div className="h-5 w-24 bg-zinc-100 rounded" />
          <div className="h-4 w-full bg-zinc-100 rounded mt-4" />
          <div className="h-4 w-5/6 bg-zinc-100 rounded" />
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
    <div className="flex-1 overflow-y-auto p-8 flex flex-col gap-6 max-w-4xl">
      <Link href={`/store/${slug}`} className="text-sm text-zinc-500 hover:text-zinc-800">
        ← Back to store
      </Link>

      <div className="flex flex-col md:flex-row gap-8">
        {getMediaUrl(product.imageUrl) ? (
          <img src={getMediaUrl(product.imageUrl)!} alt={product.name} className="w-full md:w-96 h-96 object-cover rounded-xl" />
        ) : (
          <div className="w-full md:w-96 h-96 rounded-xl bg-zinc-100 shrink-0" />
        )}

        <div className="flex flex-col gap-3">
          <h1 className="text-2xl font-semibold text-zinc-900">{product.name}</h1>
          <p className="text-xl font-bold text-zinc-900">{fmt.format(product.price)}</p>
          <p className={`text-sm ${outOfStock ? "text-zinc-400" : "text-green-600"}`}>
            {outOfStock ? "Out of stock" : "In stock"}
          </p>
          {product.description && (
            <div
              className="text-sm text-zinc-600 prose prose-sm mt-2"
              dangerouslySetInnerHTML={{ __html: product.description }}
            />
          )}
          <Button
            className="mt-4 w-full md:w-auto"
            disabled={outOfStock || isPending}
            onClick={handleAddToCart}
          >
            {isPending ? "Adding…" : outOfStock ? "Out of stock" : "Add to cart"}
          </Button>
        </div>
      </div>
    </div>
  );
}
