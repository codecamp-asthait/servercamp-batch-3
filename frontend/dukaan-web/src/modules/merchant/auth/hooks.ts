import { useMutation } from "@tanstack/react-query";
import { authApi } from "./api";
import { localStorageService } from "@/lib/local-storage.service";
import type { LoginRequest } from "./types";

export function useLogin(onSuccess: () => void) {
  return useMutation({
    mutationFn: (data: LoginRequest) => authApi.login(data),
    onSuccess: ({ token, email }) => {
      localStorageService.setToken(token);
      localStorageService.setEmail(email);
      onSuccess();
    },
  });
}
