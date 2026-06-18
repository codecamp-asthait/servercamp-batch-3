"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { useCustomerLogin } from "@/modules/store/auth/hooks";
import { localStorageService } from "@/lib/local-storage.service";
import { HttpError } from "@/lib/http";
import { Spinner } from "@/components/spinner";

function errorMessage(error: Error) {
  if (error instanceof HttpError) {
    if (error.status === 401) return "Invalid email or password";
    if (error.status === 404) return "Store not found";
  }
  return error.message;
}

export function LoginForm() {
  const { slug } = useParams<{ slug: string }>();
  const router = useRouter();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (localStorageService.getCustomerToken(slug)) {
      router.replace(redirect ?? `/store/${slug}`);
    } else {
      setReady(true);
    }
  }, [slug, router, redirect]);

  const { mutate, isPending, error } = useCustomerLogin(slug, () =>
    router.replace(redirect ?? `/store/${slug}`)
  );

  if (!ready) return <Spinner />;

  return (
    <main className="flex min-h-screen items-center justify-center">
      <form
        onSubmit={(e) => { e.preventDefault(); mutate({ email, password }); }}
        className="flex flex-col gap-4 w-80"
      >
        <h1 className="text-2xl font-semibold">Sign in</h1>
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
        {error && <p className="text-red-500 text-sm">{errorMessage(error)}</p>}
        <button
          type="submit"
          disabled={isPending}
          className="bg-black text-white rounded px-3 py-2 disabled:opacity-50"
        >
          {isPending ? "Signing in…" : "Sign in"}
        </button>
        <p className="text-sm text-center">
          No account?{" "}
          <Link href={`/store/${slug}/register`} className="underline">
            Register
          </Link>
        </p>
      </form>
    </main>
  );
}
