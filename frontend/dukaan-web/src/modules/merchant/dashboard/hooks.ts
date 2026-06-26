import { useQuery } from "@tanstack/react-query";
import { localStorageService } from "@/lib/local-storage.service";
import { dashboardApi } from "./api";

export function useMerchantProfile() {
  return useQuery({
    queryKey: ["merchant-profile"],
    queryFn: async () => {
      const profile = await dashboardApi.getProfile();
      localStorageService.setStoreName(profile.storeName);
      return profile;
    },
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}
