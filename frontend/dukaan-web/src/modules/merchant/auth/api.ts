import { http } from "@/lib/http";
import type { LoginRequest, LoginResponse } from "./types";

export const authApi = {
  login: (data: LoginRequest) =>
    http<LoginResponse>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(data),
    }),
};
