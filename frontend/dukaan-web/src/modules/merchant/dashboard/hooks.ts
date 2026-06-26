import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "./api";

export function useMerchantProfile() {
  return useQuery({
    queryKey: ["merchant-profile"],
    queryFn: () => dashboardApi.getProfile(),
    staleTime: 5 * 60 * 1000,
    retry: false,
  });
}
