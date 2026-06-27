"use client";

import { useState } from "react";
import { User, MapPin, Package, LogOut } from "lucide-react";
import { Tabs, TabsContent } from "@/components/ui/tabs";
import { ProfileTab } from "./profile-tab";
import { AddressesTab } from "./addresses-tab";
import { OrdersTab } from "./orders-tab";
import { localStorageService } from "@/lib/local-storage.service";
import { useRouter } from "next/navigation";

interface ProfilePageProps {
  slug: string;
  token: string | null;
}

type Tab = "profile" | "addresses" | "orders";

const NAV_ITEMS: { key: Tab; icon: React.ReactNode; label: string }[] = [
  { key: "profile", icon: <User size={18} />, label: "Profile" },
  { key: "addresses", icon: <MapPin size={18} />, label: "Addresses" },
  { key: "orders", icon: <Package size={18} />, label: "Orders" },
];

export function ProfilePage({ slug, token }: ProfilePageProps) {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState<Tab>("profile");

  const handleLogout = () => {
    localStorageService.removeCustomerToken(slug);
    localStorageService.removeCustomerEmail(slug);
    router.push(`/store/${slug}/login`);
  };

  return (
    <div className="xl:w-6xl md:w-3xl sm:w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">My Account</h1>
        <p className="text-gray-500 mt-1">Manage your profile, orders, and addresses.</p>
      </div>

      <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as Tab)} className="w-full">
        <div className="flex flex-col md:flex-row gap-8">
          <aside className="w-full md:w-56 shrink-0 flex flex-col gap-1">
            {NAV_ITEMS.map(({ key, icon, label }) => (
              <button
                key={key}
                onClick={() => setActiveTab(key)}
                className={`flex items-center gap-3 px-4 py-2.5 text-sm font-medium rounded-lg transition-colors w-full ${
                  activeTab === key
                    ? "bg-gray-900 text-white"
                    : "text-gray-600 hover:bg-gray-100 hover:text-gray-900"
                }`}
              >
                {icon}
                {label}
              </button>
            ))}

            <div className="mt-8 pt-6 border-t border-gray-200">
              <button
                onClick={handleLogout}
                className="flex items-center gap-3 px-4 py-2.5 text-sm font-medium text-red-600 hover:bg-red-50 rounded-lg w-full transition-colors"
              >
                <LogOut size={18} />
                Sign Out
              </button>
            </div>
          </aside>

          <div className="flex-1 min-w-0">
            <TabsContent value="profile" className="w-full"><ProfileTab slug={slug} token={token} /></TabsContent>
            <TabsContent value="addresses" className="w-full"><AddressesTab slug={slug} token={token} /></TabsContent>
            <TabsContent value="orders" className="w-full"><OrdersTab slug={slug} /></TabsContent>
          </div>
        </div>
      </Tabs>
    </div>
  );
}
