"use client";

import { useState } from "react";
import { flexRender, getCoreRowModel, useReactTable } from "@tanstack/react-table";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { useOrders } from "@/modules/merchant/orders/hooks";
import { columns } from "./columns";

const PAGE_SIZE = 10;

export function OrdersTable() {
  const [page, setPage] = useState(1);
  const { data, isLoading, isError } = useOrders(page, PAGE_SIZE);

  const table = useReactTable({
    data: data?.items ?? [],
    columns,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
  });

  return (
    <div className="p-8 flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-zinc-900">Orders</h1>
      </div>

      <div className="rounded-xl border border-zinc-200 bg-white overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            {table.getHeaderGroups().map((hg) => (
              <tr key={hg.id} className="border-b border-zinc-200 bg-zinc-50">
                {hg.headers.map((header, i) => (
                  <th
                    key={header.id}
                    className={`py-2 text-left text-xs font-medium text-zinc-500 uppercase tracking-wide ${i === 0 ? "pl-4 pr-2" : "px-4"}`}
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
                  Failed to load orders.
                </td>
              </tr>
            ) : table.getRowModel().rows.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-8 text-center text-sm text-zinc-400">
                  No orders found.
                </td>
              </tr>
            ) : (
              table.getRowModel().rows.map((row) => (
                <tr
                  key={row.id}
                  className="border-b border-zinc-100 last:border-0 hover:bg-zinc-50 transition-colors"
                >
                  {row.getVisibleCells().map((cell, i) => (
                    <td key={cell.id} className={`py-2 ${i === 0 ? "pl-4 pr-2" : "px-4"}`}>
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
          {data ? `${data.totalCount} orders` : ""}
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
    </div>
  );
}
