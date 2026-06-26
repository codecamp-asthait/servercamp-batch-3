"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { useCustomerAuthState } from "../hooks";
import { CartDrawer } from "@/modules/store/cart/components/cart-drawer";
import { NotificationBell } from "@/modules/notifications/notification-bell";

export function StorefrontHeader({ slug }: { slug: string }) {
  const { token, email, logout } = useCustomerAuthState(slug);

  return (
    <header className="sticky top-0 z-10 flex items-center justify-between px-6 py-3 border-b border-zinc-200 bg-white/80 backdrop-blur-sm">
      <span className="font-semibold">Store</span>
      <div className="flex items-center gap-3">
        <CartDrawer slug={slug} token={token} />
        <NotificationBell token={token} enabled />
        {token ? (
          <>
            <span className="text-sm text-zinc-500 hidden sm:block">{email}</span>
            <Link
              href={`/store/${slug}/orders`}
              className="text-sm px-2.5 h-7 inline-flex items-center rounded-lg hover:bg-muted transition-colors"
            >
              Orders
            </Link>
            <Button variant="ghost" size="sm" onClick={logout}>Sign out</Button>
          </>
        ) : (
          <Link href={`/store/${slug}/login`} className="text-sm px-2.5 h-7 inline-flex items-center rounded-lg hover:bg-muted transition-colors">
            Sign in
          </Link>
        )}
      </div>
    </header>
  );
}
