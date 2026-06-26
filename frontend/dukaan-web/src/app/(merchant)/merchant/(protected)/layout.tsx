"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { SidebarProvider, SidebarInset } from "@/components/ui/sidebar";
import { MerchantSidebar } from "@/modules/merchant/dashboard/components/merchant-sidebar";
import { localStorageService } from "@/lib/local-storage.service";
import { useLocalStorageToken } from "@/lib/use-local-storage";
import { http } from "@/lib/http";
import { Spinner } from "@/components/spinner";

interface MerchantProfile {
  id: string;
  storeName: string;
  slug: string;
}

export default function MerchantLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const token = useLocalStorageToken("token");
  const [storeName, setStoreName] = useState(() => localStorageService.getStoreName() ?? "");

  useEffect(() => {
    if (token === null) router.replace("/merchant/login");
  }, [router, token]);

  useEffect(() => {
    if (!token) return;
    const cached = localStorageService.getStoreName();
    if (cached) {
      setStoreName(cached);
      return;
    }
    http<MerchantProfile>("/api/merchants/profile", {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    }).then((profile) => {
      localStorageService.setStoreName(profile.storeName);
      setStoreName(profile.storeName);
    }).catch(() => {});
  }, [token]);

  if (token === undefined || !token) return <Spinner />;

  return (
    <SidebarProvider>
      <MerchantSidebar storeName={storeName} />
      <SidebarInset className="overflow-y-auto">{children}</SidebarInset>
    </SidebarProvider>
  );
}
