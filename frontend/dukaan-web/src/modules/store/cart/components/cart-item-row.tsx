"use client";

import { Minus, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { getMediaUrl } from "@/lib/utils";
import type { CartItemDto } from "../types";

const fmt = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

interface Props {
  item: CartItemDto;
  onUpdate: (productId: string, quantity: number) => void;
  onRemove: (productId: string) => void;
}

export function CartItemRow({ item, onUpdate, onRemove }: Props) {
  return (
    <>
      <div className="flex items-center gap-4 py-5">
        {getMediaUrl(item.imageUrl) ? (
          <img src={getMediaUrl(item.imageUrl)!} alt={item.productName} className="h-20 w-20 rounded-xl object-cover flex-shrink-0" />
        ) : (
          <div className="h-20 w-20 rounded-xl bg-zinc-100 flex-shrink-0" />
        )}
        <div className="flex-1 min-w-0">
          <p className="font-medium text-zinc-900 truncate">{item.productName}</p>
          <p className="text-sm text-zinc-500 mt-0.5">{fmt.format(item.unitPrice)} each</p>
          <div className="flex items-center gap-1 mt-3">
            <Button
              variant="outline"
              size="icon"
              className="h-7 w-7"
              onClick={() => onUpdate(item.productId, item.quantity - 1)}
              disabled={item.quantity <= 1}
            >
              <Minus size={12} />
            </Button>
            <span className="w-8 text-center text-sm font-medium">{item.quantity}</span>
            <Button
              variant="outline"
              size="icon"
              className="h-7 w-7"
              onClick={() => onUpdate(item.productId, item.quantity + 1)}
            >
              <Plus size={12} />
            </Button>
          </div>
        </div>
        <div className="flex flex-col items-end gap-3">
          <p className="font-semibold text-zinc-900">{fmt.format(item.subtotal)}</p>
          <Button
            variant="ghost"
            size="icon"
            className="h-7 w-7 text-zinc-400 hover:text-destructive"
            onClick={() => onRemove(item.productId)}
          >
            <Trash2 size={15} />
          </Button>
        </div>
      </div>
      <Separator />
    </>
  );
}
