import { http } from "@/lib/http";
import { localStorageService } from "@/lib/local-storage.service";

interface MerchantProfile {
  id: string;
  storeName: string;
  slug: string;
}

const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorageService.getToken()}`,
});

export const dashboardApi = {
  getProfile: () =>
    http<MerchantProfile>("/api/merchants/profile", {
      headers: authHeaders(),
    }),
};
