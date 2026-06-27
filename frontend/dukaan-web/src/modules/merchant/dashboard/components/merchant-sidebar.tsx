"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { LayoutDashboard, Package, Tag, ShoppingCart, LogOut } from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { localStorageService } from "@/lib/local-storage.service";
import { NotificationBell } from "@/modules/notifications/notification-bell";

const NAV_LINKS = [
  { label: "Dashboard", href: "/merchant/dashboard", icon: LayoutDashboard },
  { label: "Products", href: "/merchant/products", icon: Package },
  { label: "Categories", href: "/merchant/categories", icon: Tag },
  { label: "Orders", href: "/merchant/orders", icon: ShoppingCart },
];

interface MerchantSidebarProps {
  storeName: string;
}

export function MerchantSidebar({ storeName }: MerchantSidebarProps) {
  const pathname = usePathname();
  const router = useRouter();

  function handleLogout() {
    localStorageService.clear();
    router.push("/merchant/login");
  }

  return (
    <Sidebar>
      <SidebarHeader className="px-4 py-4 flex flex-row items-center justify-between">
        <p className="truncate text-lg font-semibold text-sidebar-foreground">{storeName}</p>
        {/* <NotificationBell token={localStorageService.getToken()} enabled /> */}
      </SidebarHeader>

      <SidebarContent className="px-2">
        <SidebarMenu>
          {NAV_LINKS.map(({ label, href, icon: Icon }) => (
            <SidebarMenuItem className="my-0.5" key={href}>
              <SidebarMenuButton
                render={<Link href={href} />}
                isActive={pathname === href}
              >
                <Icon />
                <span>{label}</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
          ))}
        </SidebarMenu>
      </SidebarContent>

      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton onClick={handleLogout}>
              <LogOut />
              <span>Logout</span>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
