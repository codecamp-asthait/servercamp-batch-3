import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { profileApi } from "./api";

export function useProfile(slug: string, token: string | null) {
  return useQuery({
    queryKey: ["customer-profile", slug],
    queryFn: () => profileApi.get(slug, token!),
    enabled: !!token,
  });
}

export function useUpdateProfile(slug: string, token: string | null) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: import("./types").UpdateCustomerProfileData) =>
      profileApi.update(slug, token!, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["customer-profile", slug] });
    },
  });
}
