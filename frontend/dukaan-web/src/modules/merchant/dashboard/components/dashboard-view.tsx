"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useLocalStorageToken } from "@/lib/use-local-storage";

export function DashboardView() {
  const router = useRouter();
  const token = useLocalStorageToken("token");

  useEffect(() => {
    if (token === null) router.replace("/merchant/login");
  }, [router, token]);

  return (
    <main className="p-8">
      <h1 className="text-2xl font-semibold text-zinc-900">Dashboard</h1>
    </main>
  );
}
