"use client";

import { useState } from "react";
import { flexRender, getCoreRowModel, useReactTable } from "@tanstack/react-table";
import { X } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetClose, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { useProducts, useDeleteProduct } from "@/modules/merchant/products/hooks";
import { buildColumns } from "./columns";
import { AddProductButton } from "./add-product-button";
import { ProductForm } from "./product-form";
import type { Product } from "../types";

const PAGE_SIZE = 10;

export function ProductsTable() {
  const [page, setPage] = useState(1);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [deletingProduct, setDeletingProduct] = useState<Product | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const { data, isLoading, isError } = useProducts(page, PAGE_SIZE);
  const { mutate: deleteProduct, isPending: isDeleting } = useDeleteProduct(() => setDeletingProduct(null));

  const columns = buildColumns(
    (product) => {
      setEditingProduct(product);
      setEditOpen(true);
    },
    (product) => setDeletingProduct(product),
  );

  const table = useReactTable({
    data: data?.items ?? [],
    columns,
    getCoreRowModel: getCoreRowModel(),
    enableRowSelection: true,
    manualPagination: true,
  });

  return (
    <div className="p-8 flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-zinc-900">Products</h1>
        <AddProductButton />
      </div>

      <div className="rounded-xl border border-zinc-200 bg-white overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            {table.getHeaderGroups().map((hg) => (
              <tr key={hg.id} className="border-b border-zinc-200 bg-zinc-50">
                {hg.headers.map((header, i) => (
                  <th
                    key={header.id}
                    className={`py-2 text-left text-xs font-medium text-zinc-500 uppercase tracking-wide ${i === 0 ? "pl-4 pr-2 w-8" : "px-4"}`}
                  >
                    {flexRender(header.column.columnDef.header, header.getContext())}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody>
            {isLoading ? (
              Array.from({ length: PAGE_SIZE }).map((_, i) => (
                <tr key={i} className="border-b border-zinc-100 last:border-0">
                  {columns.map((_, j) => (
                    <td key={j} className="px-4 py-2">
                      <Skeleton className="h-4 w-full" />
                    </td>
                  ))}
                </tr>
              ))
            ) : isError ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-8 text-center text-sm text-red-500">
                  Failed to load products.
                </td>
              </tr>
            ) : table.getRowModel().rows.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-8 text-center text-sm text-zinc-400">
                  No products found.
                </td>
              </tr>
            ) : (
              table.getRowModel().rows.map((row) => (
                <tr
                  key={row.id}
                  className="border-b border-zinc-100 last:border-0 hover:bg-zinc-50 transition-colors"
                >
                  {row.getVisibleCells().map((cell, i) => (
                    <td key={cell.id} className={`py-2 ${i === 0 ? "pl-4 pr-2 w-8" : i === row.getVisibleCells().length - 1 ? "pr-3 pl-1 w-10" : "px-4"}`}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="flex items-center justify-between">
        <span className="text-sm text-zinc-400">
          {data ? `${data.totalCount} products` : ""}
        </span>
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="sm"
            onClick={() => setPage((p) => p - 1)}
            disabled={!data?.hasPreviousPage}
          >
            Previous
          </Button>
          <span className="text-sm text-zinc-500">
            Page {data?.pageNumber ?? 1} of {data?.totalPages ?? 1}
          </span>
          <Button
            variant="outline"
            size="sm"
            onClick={() => setPage((p) => p + 1)}
            disabled={!data?.hasNextPage}
          >
            Next
          </Button>
        </div>
      </div>

      {/* Edit Product Sheet */}
      <Sheet open={editOpen} onOpenChange={(open) => { setEditOpen(open); if (!open) setEditingProduct(null); }}>
        <SheetContent
          side="right"
          showCloseButton={false}
          className="flex flex-col p-0 !w-[560px] !inset-y-3 !right-3 !h-[calc(100vh-24px)] rounded-xl shadow-2xl"
        >
          <SheetHeader className="flex flex-row items-center justify-between border-b px-6 py-4 space-y-0">
            <SheetTitle>Edit product</SheetTitle>
            <SheetClose
              render={
                <button
                  type="button"
                  className="rounded p-1 text-zinc-400 hover:text-zinc-700 hover:bg-zinc-100 transition-colors"
                />
              }
            >
              <X size={15} />
            </SheetClose>
          </SheetHeader>
          {editingProduct && (
            <ProductForm
              key={editingProduct.id}
              product={editingProduct}
              onSuccess={() => { setEditOpen(false); setEditingProduct(null); }}
              onCancel={() => { setEditOpen(false); setEditingProduct(null); }}
            />
          )}
        </SheetContent>
      </Sheet>

      {/* Delete confirmation dialog */}
      {deletingProduct && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="bg-white rounded-xl shadow-2xl p-6 w-[400px]">
            <h2 className="text-lg font-semibold text-zinc-900 mb-2">Delete product</h2>
            <p className="text-sm text-zinc-500 mb-6">
              Are you sure you want to delete &quot;{deletingProduct.name}&quot;? This action cannot be undone.
            </p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeletingProduct(null)} disabled={isDeleting}>
                Cancel
              </Button>
              <Button
                className="bg-red-600 text-white hover:bg-red-700"
                disabled={isDeleting}
                onClick={() => deleteProduct(deletingProduct.id)}
              >
                {isDeleting ? "Deleting…" : "Delete"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
