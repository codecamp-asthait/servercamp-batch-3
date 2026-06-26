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

const NAV_LINKS = [
  { label: "Dashboard", href: "/merchant/dashboard", icon: LayoutDashboard },
  { label: "Products", href: "/merchant/products", icon: Package },
  { label: "Categories", href: "/merchant/categories", icon: Tag },
  { label: "Orders", href: "/merchant/orders", icon: ShoppingCart },
];

interface MerchantSidebarProps {
  storeName: string;
  email: string;
}

export function MerchantSidebar({ storeName, email }: MerchantSidebarProps) {
  const pathname = usePathname();
  const router = useRouter();

  function handleLogout() {
    localStorageService.clear();
    router.push("/merchant/login");
  }

  return (
    <Sidebar>
      <SidebarHeader className="px-4 py-4">
        <p className="truncate text-sm font-medium text-sidebar-foreground">{storeName}</p>
        <p className="truncate text-xs text-sidebar-foreground/60">{email}</p>
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
