const BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export class HttpError extends Error {
  constructor(public status: number) {
    super(`HTTP error ${status}`);
  }
}

export async function http<T>(
  path: string,
  options?: RequestInit & { baseUrl?: string }
): Promise<T> {
  const base = options?.baseUrl ?? BASE_URL;
  const isFormData = options?.body instanceof FormData || options?.body instanceof Blob;

  const res = await fetch(`${base}${path}`, {
    ...(isFormData ? {} : { headers: { "Content-Type": "application/json" } }),
    ...options,
  });

  if (!res.ok) throw new HttpError(res.status);

  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text) as T;
}
