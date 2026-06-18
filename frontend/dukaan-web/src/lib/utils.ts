import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

const MINIO_BASE = process.env.NEXT_PUBLIC_MINIO_URL ?? "http://localhost:9000/dukaan-media";

export function getMediaUrl(imageUrl: string | null, variant: "display" | "thumbnail" | "original" = "display"): string | null {
  if (!imageUrl) return null;
  return `${MINIO_BASE}/${imageUrl}/${variant}.webp`;
}
