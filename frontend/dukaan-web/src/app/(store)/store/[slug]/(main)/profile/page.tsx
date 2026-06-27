"use client";

import { useParams } from "next/navigation";
import { useCustomerAuthState } from "@/modules/store/auth/hooks";
import { ProfilePage } from "@/modules/store/profile/components/profile-page";
import { Spinner } from "@/components/spinner";

export default function ProfileRoute() {
  const params = useParams<{ slug: string }>();
  const slug = params.slug;
  const { token, pending } = useCustomerAuthState(slug);

  if (pending) return <Spinner />;
  if (!token) return <div className="p-6 text-sm text-muted-foreground">Please sign in.</div>;

  return <ProfilePage slug={slug} token={token} />;
}
