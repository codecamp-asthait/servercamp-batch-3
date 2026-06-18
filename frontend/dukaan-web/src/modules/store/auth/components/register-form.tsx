"use client";

import { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { useCustomerRegister } from "@/modules/store/auth/hooks";
import { HttpError } from "@/lib/http";

function errorMessage(error: Error) {
  if (error instanceof HttpError && error.status === 409)
    return "An account with this email already exists";
  return error.message;
}

export function RegisterForm() {
  const { slug } = useParams<{ slug: string }>();
  const router = useRouter();
  const [form, setForm] = useState({
    firstName: "", lastName: "", email: "", password: "", phone: "",
  });

  const { mutate, isPending, error } = useCustomerRegister(slug, () =>
    router.replace(`/store/${slug}/login`)
  );

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  return (
    <main className="flex min-h-screen items-center justify-center">
      <form
        onSubmit={(e) => { e.preventDefault(); mutate(form); }}
        className="flex flex-col gap-4 w-80"
      >
        <h1 className="text-2xl font-semibold">Create account</h1>
        <input name="firstName" placeholder="First name" value={form.firstName} onChange={handleChange} required className="border rounded px-3 py-2" />
        <input name="lastName" placeholder="Last name" value={form.lastName} onChange={handleChange} required className="border rounded px-3 py-2" />
        <input name="email" type="email" placeholder="Email" value={form.email} onChange={handleChange} required className="border rounded px-3 py-2" />
        <input name="password" type="password" placeholder="Password" value={form.password} onChange={handleChange} required className="border rounded px-3 py-2" />
        <input name="phone" type="tel" placeholder="Phone" value={form.phone} onChange={handleChange} required className="border rounded px-3 py-2" />
        {error && <p className="text-red-500 text-sm">{errorMessage(error)}</p>}
        <button
          type="submit"
          disabled={isPending}
          className="bg-black text-white rounded px-3 py-2 disabled:opacity-50"
        >
          {isPending ? "Creating account…" : "Create account"}
        </button>
        <p className="text-sm text-center">
          Already have an account?{" "}
          <Link href={`/store/${slug}/login`} className="underline">
            Sign in
          </Link>
        </p>
      </form>
    </main>
  );
}
