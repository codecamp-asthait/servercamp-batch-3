import { OrderHistoryView } from "@/modules/store/orders/components/order-history-view";

export default async function OrdersPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  return <OrderHistoryView slug={slug} />;
}
