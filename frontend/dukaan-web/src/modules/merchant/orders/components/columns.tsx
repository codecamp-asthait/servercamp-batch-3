"use client";

import Link from "next/link";
import { createColumnHelper } from "@tanstack/react-table";
import type { MerchantOrderSummary } from "../types";

const col = createColumnHelper<MerchantOrderSummary>();

function StatusBadge({ status }: { status: string }) {
  const colors: Record<string, string> = {
    Pending: "bg-yellow-100 text-yellow-700",
    Confirmed: "bg-blue-100 text-blue-700",
    Cancelled: "bg-red-100 text-red-700",
  };
  const color = colors[status] ?? "bg-zinc-100 text-zinc-500";
  return (
    <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${color}`}>
      {status}
    </span>
  );
}

export const columns = [
  col.accessor("orderNumber", {
    header: "Order",
    cell: (info) => {
      const id = info.row.original.id;
      return (
        <Link href={`/merchant/orders/${id}`} className="text-sm font-medium text-blue-600 hover:underline">
          {info.getValue()}
        </Link>
      );
    },
  }),
  col.accessor("customerName", {
    header: "Customer",
    cell: (info) => <span className="text-sm text-zinc-900">{info.getValue()}</span>,
  }),
  col.accessor("status", {
    header: "Status",
    cell: (info) => <StatusBadge status={info.getValue()} />,
  }),
  col.accessor("itemCount", {
    header: "Items",
    cell: (info) => <span className="text-sm text-zinc-600">{info.getValue()}</span>,
  }),
  col.accessor("total", {
    header: "Total",
    cell: (info) => (
      <span className="text-sm text-zinc-600">${info.getValue().toFixed(2)}</span>
    ),
  }),
  col.accessor("createdAt", {
    header: "Date",
    cell: (info) => (
      <span className="text-sm text-zinc-500">
        {new Date(info.getValue()).toLocaleDateString()}
      </span>
    ),
  }),
];
