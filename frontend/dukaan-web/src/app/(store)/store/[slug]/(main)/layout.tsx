import { StorefrontHeader } from "@/modules/store/auth/components/storefront-header";

export default function StoreMainLayout({
  children,
  params,
}: {
  children: React.ReactNode;
  params: Promise<{ slug: string }>;
}) {
  return (
    <div className="flex flex-col h-screen overflow-hidden">
      <StorefrontHeaderWrapper params={params} />
      {children}
    </div>
  );
}

async function StorefrontHeaderWrapper({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  return <StorefrontHeader slug={slug} />;
}
