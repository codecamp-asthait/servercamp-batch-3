import { ProductCard } from "./product-card";
import type { StorefrontProduct } from "../types";

interface ProductGridProps {
  products: StorefrontProduct[];
  isLoading: boolean;
}

export function ProductGrid({ products, isLoading }: ProductGridProps) {
  if (isLoading) {
    return (
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <div key={i} className="rounded-xl border border-zinc-200 bg-zinc-100 h-64 animate-pulse" />
        ))}
      </div>
    );
  }

  if (products.length === 0) {
    return <p className="text-sm text-zinc-400 py-12 text-center">No products found.</p>;
  }

  return (
    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
      {products.map((p) => <ProductCard key={p.id} product={p} />)}
    </div>
  );
}
