"use client";

import { useRef, useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { MultiSelect } from "@/components/multi-select";
import { RichTextEditor } from "@/components/rich-text-editor";
import { useCreateProduct, useUpdateProduct } from "@/modules/merchant/products/hooks";
import { useCategoriesDropdown } from "@/modules/merchant/categories/hooks";
import { mediaApi } from "@/modules/merchant/products/api";
import type { Product } from "@/modules/merchant/products/types";

interface ProductFormProps {
  product?: Product;
  onSuccess: () => void;
  onCancel: () => void;
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex flex-col gap-1.5">
      <label className="text-sm font-medium text-zinc-700">{label}</label>
      {children}
    </div>
  );
}

const inputCls = "rounded-md border border-zinc-200 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-zinc-300";

async function uploadFile(
  file: File,
  onProgress: (pct: number) => void
): Promise<string> {
  const { mediaId, chunkSize, totalChunks } = await mediaApi.initiateUpload(
    file.name,
    file.type,
    file.size
  );

  for (let i = 0; i < totalChunks; i++) {
    const start = i * chunkSize;
    const chunk = file.slice(start, start + chunkSize);
    await mediaApi.uploadChunk(mediaId, i, chunk);
    onProgress(Math.round(((i + 1) / totalChunks) * 100));
  }

  await mediaApi.completeUpload(mediaId);
  return mediaId;
}

export function ProductForm({ product, onSuccess, onCancel }: ProductFormProps) {
  const isEditing = !!product;

  const [name, setName] = useState(product?.name ?? "");
  const [description, setDescription] = useState(product?.description ?? "");
  const [price, setPrice] = useState(product?.price.toString() ?? "");
  const [stockQuantity, setStockQuantity] = useState(product?.stockQuantity.toString() ?? "");
  const [isActive, setIsActive] = useState(product?.isActive ?? true);
  const [categoryIds, setCategoryIds] = useState<string[]>(product?.categoryIds ?? []);
  const [mediaId, setMediaId] = useState<string | null>(null);
  const [uploadProgress, setUploadProgress] = useState<number | null>(null);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const { data: categories = [] } = useCategoriesDropdown();
  const { mutate: createProduct, isPending: isCreating, error: createError } = useCreateProduct(onSuccess);
  const { mutate: updateProduct, isPending: isUpdating, error: updateError } = useUpdateProduct(onSuccess);

  useEffect(() => {
    if (product) {
      setName(product.name);
      setDescription(product.description ?? "");
      setPrice(product.price.toString());
      setStockQuantity(product.stockQuantity.toString());
      setIsActive(product.isActive);
      setCategoryIds(product.categoryIds);
    }
  }, [product]);

  async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploadError(null);
    setMediaId(null);
    setUploadProgress(0);
    try {
      const id = await uploadFile(file, setUploadProgress);
      setMediaId(id);
    } catch (err) {
      setUploadError(err instanceof Error ? err.message : "Upload failed");
      setUploadProgress(null);
    }
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (isEditing && product) {
      updateProduct({
        id: product.id,
        data: {
          name,
          description: description || null,
          price: parseFloat(price),
          pendingMediaId: mediaId,
          stockQuantity: parseInt(stockQuantity),
          isActive,
        },
      });
    } else {
      createProduct({
        name,
        description: description || null,
        price: parseFloat(price),
        pendingMediaId: mediaId,
        stockQuantity: parseInt(stockQuantity),
        categoryIds,
      });
    }
  }

  const error = createError ?? updateError;
  const isPending = isCreating || isUpdating;
  const categoryOptions = categories.map((c) => ({ value: c.id, label: c.name }));

  return (
    <form onSubmit={handleSubmit} className="flex flex-col flex-1 overflow-hidden">
      <div className="flex flex-col gap-5 overflow-y-auto p-6 flex-1">
        <Field label="Name">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            placeholder="Product name"
            className={inputCls}
          />
        </Field>

        <Field label="Price">
          <input
            type="number"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
            min={0}
            step={0.01}
            placeholder="0.00"
            className={inputCls}
          />
        </Field>

        <Field label="Stock Quantity">
          <input
            type="number"
            value={stockQuantity}
            onChange={(e) => setStockQuantity(e.target.value)}
            required
            min={0}
            placeholder="0"
            className={inputCls}
          />
        </Field>

        {isEditing && (
          <Field label="Status">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                checked={isActive}
                onChange={(e) => setIsActive(e.target.checked)}
                className="rounded border-zinc-300"
              />
              <span className="text-sm text-zinc-600">Product is active</span>
            </label>
          </Field>
        )}

        {!isEditing && (
          <Field label="Categories">
            <MultiSelect
              options={categoryOptions}
              value={categoryIds}
              onChange={setCategoryIds}
              placeholder="Select categories"
            />
          </Field>
        )}

        <Field label="Image">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            onChange={handleFileChange}
            className="hidden"
          />
          <button
            type="button"
            onClick={() => fileInputRef.current?.click()}
            disabled={uploadProgress !== null && uploadProgress < 100}
            className="rounded-md border border-dashed border-zinc-300 px-3 py-4 text-sm text-zinc-500 hover:border-zinc-400 hover:text-zinc-700 disabled:opacity-50 text-center"
          >
            {uploadProgress === null && !mediaId && "Click to choose an image"}
            {uploadProgress !== null && uploadProgress < 100 && `Uploading… ${uploadProgress}%`}
            {mediaId && uploadProgress === 100 && "✓ Image uploaded"}
          </button>
          {uploadError && <p className="text-xs text-red-500">{uploadError}</p>}
        </Field>

        <Field label="Description">
          <RichTextEditor value={description} onChange={setDescription} />
        </Field>

        {error && (
          <p className="text-sm text-red-500">{error.message}</p>
        )}
      </div>

      <div className="flex justify-end gap-2 border-t border-zinc-200 px-6 py-4">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" disabled={isPending || (uploadProgress !== null && uploadProgress < 100)} className="bg-zinc-900 text-white hover:bg-zinc-700">
          {isPending ? "Saving…" : isEditing ? "Save changes" : "Add product"}
        </Button>
      </div>
    </form>
  );
}
