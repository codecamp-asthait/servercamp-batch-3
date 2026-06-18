"use client";

import { createColumnHelper } from "@tanstack/react-table";
import { ChevronRight, ChevronDown, MoreHorizontal } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import type { Category } from "../types";

const col = createColumnHelper<Category>();

export const columns = [
  col.accessor("name", {
    header: "Name",
    cell: ({ row, getValue }) => (
      <div
        className="flex items-center gap-1 font-medium text-zinc-900"
        style={{ paddingLeft: row.depth * 24 }}
      >
        {row.getCanExpand() ? (
          <button
            onClick={row.getToggleExpandedHandler()}
            className="text-zinc-400 hover:text-zinc-700"
          >
            {row.getIsExpanded() ? <ChevronDown size={14} /> : <ChevronRight size={14} />}
          </button>
        ) : (
          <span className="w-[14px] text-center text-zinc-300 text-xs">•</span>
        )}
        {getValue()}
      </div>
    ),
  }),
  col.accessor("description", {
    header: "Description",
    cell: (info) => (
      <span className="text-zinc-500 truncate block max-w-xs">
        {info.getValue() ?? "—"}
      </span>
    ),
  }),
  col.display({
    id: "actions",
    header: () => <span className="sr-only">Actions</span>,
    cell: () => (
      <div className="flex justify-end">
        <DropdownMenu>
          <DropdownMenuTrigger render={<Button variant="ghost" size="icon" className="h-7 w-7" />}>
            <MoreHorizontal size={14} />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem>Edit</DropdownMenuItem>
            <DropdownMenuItem className="text-red-600">Delete</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    ),
  }),
];
