import { OrderDetailView } from "@/modules/store/orders/components/order-detail-view";

export default async function OrderDetailPage({
  params,
}: {
  params: Promise<{ slug: string; id: string }>;
}) {
  const { slug, id } = await params;
  return <OrderDetailView slug={slug} orderId={id} />;
}
