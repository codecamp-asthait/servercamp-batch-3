"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { SidebarProvider, SidebarInset } from "@/components/ui/sidebar";
import { MerchantSidebar } from "@/modules/merchant/dashboard/components/merchant-sidebar";
import { useLocalStorageToken } from "@/lib/use-local-storage";
import { useMerchantProfile } from "@/modules/merchant/dashboard/hooks";
import { Spinner } from "@/components/spinner";

export default function MerchantLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const token = useLocalStorageToken("token");
  const { data: profile } = useMerchantProfile();

  useEffect(() => {
    if (token === null) router.replace("/merchant/login");
  }, [router, token]);

  if (token === undefined || !token) return <Spinner />;

  return (
    <SidebarProvider>
      <MerchantSidebar storeName={profile?.storeName ?? ""} />
      <SidebarInset className="overflow-y-auto">{children}</SidebarInset>
    </SidebarProvider>
  );
}
