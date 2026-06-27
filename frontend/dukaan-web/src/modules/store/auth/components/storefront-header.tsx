"use client";

import Link from "next/link";
import { useCustomerAuthState } from "../hooks";
import { CartDrawer } from "@/modules/store/cart/components/cart-drawer";
import { NotificationBell } from "@/modules/notifications/notification-bell";

export function StorefrontHeader({ slug }: { slug: string }) {
  const { token, email } = useCustomerAuthState(slug);

  const storeName = slug
    .split("-")
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ");

  return (
    <header className="sticky top-0 z-10 flex items-center justify-between px-6 py-3 border-b border-gray-200 bg-white">
      <Link href={`/store/${slug}`} className="font-bold text-xl tracking-tight hover:opacity-80 transition-opacity">
        {storeName}
      </Link>
      <div className="flex items-center gap-4">
        <CartDrawer slug={slug} token={token} />
        <NotificationBell token={token} enabled />
        {token ? (
          <Link href={`/store/${slug}/profile`}>
            <div className="h-8 w-8 bg-gray-900 text-white rounded-full flex items-center justify-center font-medium text-sm cursor-pointer">
              {email ? email.charAt(0).toUpperCase() : "U"}
            </div>
          </Link>
        ) : (
          <Link href={`/store/${slug}/login`} className="text-sm px-2.5 h-7 inline-flex items-center rounded-lg hover:bg-gray-100 transition-colors">
            Sign in
          </Link>
        )}
      </div>
    </header>
  );
}
