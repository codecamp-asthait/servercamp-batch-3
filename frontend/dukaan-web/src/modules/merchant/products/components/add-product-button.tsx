"use client";

import { useState } from "react";
import { X } from "lucide-react";
import { Sheet, SheetContent, SheetClose, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { ProductForm } from "./product-form";

export function AddProductButton() {
  const [open, setOpen] = useState(false);

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger render={<Button size="sm" className="bg-zinc-900 text-white hover:bg-zinc-700" />}>
        Add product
      </SheetTrigger>
      <SheetContent
        side="right"
        showCloseButton={false}
        className="flex flex-col p-0 !w-[560px] !inset-y-3 !right-3 !h-[calc(100vh-24px)] rounded-xl shadow-2xl"
      >
        <SheetHeader className="flex flex-row items-center justify-between border-b px-6 py-4 space-y-0">
          <SheetTitle>Add product</SheetTitle>
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
        <ProductForm onSuccess={() => setOpen(false)} onCancel={() => setOpen(false)} />
      </SheetContent>
    </Sheet>
  );
}
