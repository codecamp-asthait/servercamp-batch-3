"use client";

import { useState } from "react";
import { useParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { CategoryFilter } from "./category-filter";
import { ProductGrid } from "./product-grid";
import {
  useStorefrontCategories,
  useStorefrontProducts,
  useStorefrontProductsByCategory,
} from "../hooks";
import { HttpError } from "@/lib/http";

export function StorefrontView() {
  const { slug } = useParams<{ slug: string }>();
  const [page, setPage] = useState(1);
  const [activeCategoryId, setActiveCategoryId] = useState<string | null>(null);

  const { data: categoriesData } = useStorefrontCategories(slug);

  const allProducts = useStorefrontProducts(slug, page);
  const categoryProducts = useStorefrontProductsByCategory(slug, activeCategoryId ?? "", page);

  const { data, isLoading, error } = activeCategoryId ? categoryProducts : allProducts;

  function handleCategorySelect(id: string | null) {
    setActiveCategoryId(id);
    setPage(1);
  }

  if (error instanceof HttpError && error.status === 404) {
    return <p className="p-8 text-sm text-zinc-500">Store not found.</p>;
  }

  return (
    <div className="flex flex-1 overflow-hidden">
      <aside className="hidden md:flex flex-col w-52 shrink-0 border-r border-zinc-200 p-4 gap-2 overflow-y-auto">
        <p className="text-xs font-semibold uppercase tracking-wide text-zinc-400 mb-1">Categories</p>
        <CategoryFilter
          categories={categoriesData?.items ?? []}
          activeId={activeCategoryId}
          onSelect={handleCategorySelect}
        />
      </aside>

      <main className="flex-1 p-6 flex flex-col gap-6 overflow-y-auto">
        <ProductGrid products={data?.items ?? []} isLoading={isLoading} />

        <div className="flex items-center justify-between">
          <span className="text-sm text-zinc-400">
            {data ? `${data.totalCount} products` : ""}
          </span>
          <div className="flex items-center gap-3">
            <Button variant="outline" size="sm" onClick={() => setPage((p) => p - 1)} disabled={!data?.hasPreviousPage}>
              Previous
            </Button>
            <span className="text-sm text-zinc-500">
              Page {data?.pageNumber ?? 1} of {data?.totalPages ?? 1}
            </span>
            <Button variant="outline" size="sm" onClick={() => setPage((p) => p + 1)} disabled={!data?.hasNextPage}>
              Next
            </Button>
          </div>
        </div>
      </main>
    </div>
  );
}
