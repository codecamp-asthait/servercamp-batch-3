"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useLogin } from "@/modules/merchant/auth/hooks";
import { useLocalStorageToken } from "@/lib/use-local-storage";
import { HttpError } from "@/lib/http";
import { Spinner } from "@/components/spinner";

export function LoginForm() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const token = useLocalStorageToken("token");

  useEffect(() => {
    if (token) router.replace("/merchant/dashboard");
  }, [router, token]);

  const { mutate, isPending, error } = useLogin(() =>
    router.replace("/merchant/dashboard")
  );

  if (token === undefined || token) return <Spinner />;

  return (
    <main className="flex min-h-screen items-center justify-center">
      <form
        onSubmit={(e) => { e.preventDefault(); mutate({ email, password }); }}
        className="flex flex-col gap-4 w-80"
      >
        <h1 className="text-2xl font-semibold">Merchant Login</h1>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="border rounded px-3 py-2"
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          className="border rounded px-3 py-2"
        />
        {error && (
          <p className="text-red-500 text-sm">
            {error instanceof HttpError && error.status === 401
              ? "Invalid email or password"
              : error.message}
          </p>
        )}
        <button
          type="submit"
          disabled={isPending}
          className="bg-black text-white rounded px-3 py-2 disabled:opacity-50"
        >
          {isPending ? "Signing in…" : "Sign in"}
        </button>
      </form>
    </main>
  );
}
