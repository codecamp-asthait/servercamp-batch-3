import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { cartApi } from "./api";
import type { CartDto, AddCartItemRequest } from "./types";

const cartKey = (slug: string) => ["storefront", slug, "cart"];

export function useCart(slug: string, token: string | null) {
  return useQuery({
    queryKey: cartKey(slug),
    queryFn: () => cartApi.getCart(slug, token!),
    enabled: !!token,
  });
}

export function useAddCartItem(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: AddCartItemRequest) => cartApi.addItem(slug, token, body),
    onSuccess: () => qc.invalidateQueries({ queryKey: cartKey(slug) }),
  });
}

export function useUpdateCartItem(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ productId, quantity }: { productId: string; quantity: number }) =>
      cartApi.updateItem(slug, token, productId, { quantity }),
    onMutate: async ({ productId, quantity }) => {
      await qc.cancelQueries({ queryKey: cartKey(slug) });
      const previous = qc.getQueryData<CartDto>(cartKey(slug));
      qc.setQueryData<CartDto>(cartKey(slug), (old) => {
        if (!old) return old;
        return {
          ...old,
          items: old.items.map((item) =>
            item.productId === productId
              ? { ...item, quantity, subtotal: item.unitPrice * quantity }
              : item
          ),
          total: old.items.reduce(
            (sum, item) =>
              item.productId === productId
                ? sum + item.unitPrice * quantity
                : sum + item.subtotal,
            0
          ),
        };
      });
      return { previous };
    },
    onError: (_err, _vars, ctx) => {
      if (ctx?.previous) qc.setQueryData(cartKey(slug), ctx.previous);
    },
    onSuccess: (data) => qc.setQueryData<CartDto>(cartKey(slug), data),
  });
}

export function useRemoveCartItem(slug: string, token: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (productId: string) => cartApi.removeItem(slug, token, productId),
    onMutate: async (productId) => {
      await qc.cancelQueries({ queryKey: cartKey(slug) });
      const previous = qc.getQueryData<CartDto>(cartKey(slug));
      qc.setQueryData<CartDto>(cartKey(slug), (old) => {
        if (!old) return old;
        const items = old.items.filter((i) => i.productId !== productId);
        return { items, total: items.reduce((sum, i) => sum + i.subtotal, 0) };
      });
      return { previous };
    },
    onError: (_err, _vars, ctx) => {
      if (ctx?.previous) qc.setQueryData(cartKey(slug), ctx.previous);
    },
    onSuccess: (data) => qc.setQueryData<CartDto>(cartKey(slug), data),
  });
}
