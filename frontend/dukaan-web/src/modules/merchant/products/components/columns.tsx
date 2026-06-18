"use client";

import { createColumnHelper } from "@tanstack/react-table";
import { getMediaUrl } from "@/lib/utils";
import type { Product } from "../types";

const col = createColumnHelper<Product>();

function StatusBadge({ active }: { active: boolean }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${
        active
          ? "bg-green-100 text-green-700"
          : "bg-zinc-100 text-zinc-500"
      }`}
    >
      {active ? "Active" : "Draft"}
    </span>
  );
}

export const columns = [
  col.display({
    id: "select",
    header: ({ table }) => (
      <input
        type="checkbox"
        className="rounded border-zinc-300"
        checked={table.getIsAllRowsSelected()}
        onChange={table.getToggleAllRowsSelectedHandler()}
      />
    ),
    cell: ({ row }) => (
      <input
        type="checkbox"
        className="rounded border-zinc-300"
        checked={row.getIsSelected()}
        onChange={row.getToggleSelectedHandler()}
      />
    ),
    size: 32,
  }),
  col.accessor("name", {
    header: "Product",
    cell: (info) => (
      <div className="flex items-center gap-3">
        {getMediaUrl(info.row.original.imageUrl) ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={getMediaUrl(info.row.original.imageUrl)!}
            alt={info.getValue()}
            className="h-8 w-8 rounded-md object-cover border border-zinc-200"
          />
        ) : (
          <div className="h-8 w-8 rounded-md bg-zinc-100 border border-zinc-200" />
        )}
        <span className="text-sm font-medium text-zinc-900">{info.getValue()}</span>
      </div>
    ),
  }),
  col.accessor("isActive", {
    header: "Status",
    cell: (info) => <StatusBadge active={info.getValue()} />,
  }),
  col.accessor("stockQuantity", {
    header: "Inventory",
    cell: (info) => {
      const qty = info.getValue();
      return (
        <span className={`text-sm ${qty === 0 ? "text-red-500" : "text-zinc-600"}`}>
          {qty === 0 ? "0 in stock" : `${qty} in stock`}
        </span>
      );
    },
  }),
  col.accessor("price", {
    header: "Price",
    cell: (info) => (
      <span className="text-sm text-zinc-600">${info.getValue().toFixed(2)}</span>
    ),
  }),
];
