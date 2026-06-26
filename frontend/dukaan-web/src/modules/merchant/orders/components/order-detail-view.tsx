"use client";

import { useParams, useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { useOrder, useUpdateOrderStatus } from "../hooks";

const STATUS_OPTIONS = ["Pending", "Confirmed", "Cancelled"] as const;

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

export function OrderDetailView() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const { data: order, isLoading, isError } = useOrder(id);
  const { mutate: updateStatus, isPending } = useUpdateOrderStatus();

  if (isLoading) return <div className="p-8 text-sm text-zinc-500">Loading order...</div>;
  if (isError || !order) return <div className="p-8 text-sm text-red-500">Failed to load order.</div>;

  const handleStatusChange = (newStatus: string) => {
    updateStatus({ id, data: { newStatus } });
  };

  return (
    <div className="p-8 max-w-4xl mx-auto flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="outline" size="sm" onClick={() => router.push("/merchant/orders")}>
            &larr; Back
          </Button>
          <h1 className="text-xl font-semibold text-zinc-900">{order.orderNumber}</h1>
          <StatusBadge status={order.status} />
        </div>
        <div className="flex items-center gap-2">
          {STATUS_OPTIONS.map((s) =>
            s !== order.status ? (
              <Button
                key={s}
                variant="outline"
                size="sm"
                onClick={() => handleStatusChange(s)}
                disabled={isPending}
              >
                Mark {s}
              </Button>
            ) : null
          )}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-6">
        <div className="rounded-xl border border-zinc-200 bg-white p-4">
          <h2 className="text-sm font-medium text-zinc-500 mb-2">Customer</h2>
          <p className="text-sm text-zinc-900">{order.customerName}</p>
        </div>
        <div className="rounded-xl border border-zinc-200 bg-white p-4">
          <h2 className="text-sm font-medium text-zinc-500 mb-2">Delivery Address</h2>
          <p className="text-sm text-zinc-900">
            {order.deliveryAddress.street}, {order.deliveryAddress.city},{" "}
            {order.deliveryAddress.district} {order.deliveryAddress.postalCode}
          </p>
          <p className="text-sm text-zinc-500">{order.deliveryAddress.phone}</p>
        </div>
      </div>

      <div className="rounded-xl border border-zinc-200 bg-white overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-zinc-200 bg-zinc-50">
              <th className="px-4 py-2 text-left text-xs font-medium text-zinc-500 uppercase tracking-wide">Product</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-zinc-500 uppercase tracking-wide">Price</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-zinc-500 uppercase tracking-wide">Qty</th>
              <th className="px-4 py-2 text-right text-xs font-medium text-zinc-500 uppercase tracking-wide">Subtotal</th>
            </tr>
          </thead>
          <tbody>
            {order.items.map((item) => (
              <tr key={item.productId} className="border-b border-zinc-100 last:border-0">
                <td className="px-4 py-3 text-sm text-zinc-900">{item.productName}</td>
                <td className="px-4 py-3 text-sm text-zinc-600">${item.unitPrice.toFixed(2)}</td>
                <td className="px-4 py-3 text-sm text-zinc-600">{item.quantity}</td>
                <td className="px-4 py-3 text-sm text-zinc-900 text-right">${item.subtotal.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
          <tfoot>
            <tr className="border-t border-zinc-200">
              <td colSpan={3} className="px-4 py-3 text-sm font-medium text-zinc-900 text-right">Total</td>
              <td className="px-4 py-3 text-sm font-semibold text-zinc-900 text-right">${order.total.toFixed(2)}</td>
            </tr>
          </tfoot>
        </table>
      </div>

      <div className="text-sm text-zinc-400">
        Placed on {new Date(order.createdAt).toLocaleDateString()} at{" "}
        {new Date(order.createdAt).toLocaleTimeString()}
      </div>
    </div>
  );
}
