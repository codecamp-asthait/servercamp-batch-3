import { useEffect, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { customerAuthApi } from "./api";
import { localStorageService } from "@/lib/local-storage.service";
import type { CustomerLoginRequest, CustomerRegisterRequest } from "./types";

export function useCustomerLogin(slug: string, onSuccess: () => void) {
  return useMutation({
    mutationFn: (data: CustomerLoginRequest) => customerAuthApi.login(slug, data),
    onSuccess: ({ token, email }) => {
      localStorageService.setCustomerToken(slug, token);
      localStorageService.setCustomerEmail(slug, email);
      onSuccess();
    },
  });
}

export function useCustomerRegister(slug: string, onSuccess: () => void) {
  return useMutation({
    mutationFn: (data: CustomerRegisterRequest) => customerAuthApi.register(slug, data),
    onSuccess,
  });
}

export function useCustomerAuthState(slug: string) {
  const [state, setState] = useState<{
    token: string | null;
    email: string | null;
    pending: boolean;
  }>({
    token: null,
    email: null,
    pending: true,
  });

  useEffect(() => {
    setState({
      token: localStorageService.getCustomerToken(slug),
      email: localStorageService.getCustomerEmail(slug),
      pending: false,
    });
  }, [slug]);

  const logout = () => {
    localStorageService.removeCustomerToken(slug);
    localStorageService.removeCustomerEmail(slug);
    setState({ token: null, email: null, pending: false });
  };

  return { ...state, logout };
}