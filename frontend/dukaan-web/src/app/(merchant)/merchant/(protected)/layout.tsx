"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { SidebarProvider, SidebarInset } from "@/components/ui/sidebar";
import { MerchantSidebar } from "@/modules/merchant/dashboard/components/merchant-sidebar";
import { localStorageService } from "@/lib/local-storage.service";
import { useLocalStorageToken } from "@/lib/use-local-storage";
import { Spinner } from "@/components/spinner";

export default function MerchantLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const token = useLocalStorageToken("token");
  const [email] = useState(() => localStorageService.getEmail() ?? "");

  useEffect(() => {
    if (token === null) router.replace("/merchant/login");
  }, [router, token]);

  if (token === undefined || !token) return <Spinner />;

  return (
    <SidebarProvider>
      <MerchantSidebar email={email} />
      <SidebarInset className="overflow-y-auto">{children}</SidebarInset>
    </SidebarProvider>
  );
}
