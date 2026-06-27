"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Camera, ShieldCheck } from "lucide-react";
import { useProfile, useUpdateProfile } from "../hooks";
import type { UpdateCustomerProfileData } from "../types";

interface ProfileTabProps {
  slug: string;
  token: string | null;
}

export function ProfileTab({ slug, token }: ProfileTabProps) {
  const { data: profile, isLoading } = useProfile(slug, token);
  const updateMutation = useUpdateProfile(slug, token);
  const { register, handleSubmit, reset } = useForm<UpdateCustomerProfileData>();

  useEffect(() => {
    if (profile) reset({ firstName: profile.firstName, lastName: profile.lastName, phone: profile.phone ?? "" });
  }, [profile, reset]);

  if (isLoading || !profile) return <div className="text-sm text-gray-500">Loading...</div>;

  return (
    <div className="w-full bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
      <div className="p-6 border-b border-gray-200 flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-gray-900">Personal Information</h2>
          <p className="text-sm text-gray-500 mt-1">Update your personal details and contact information.</p>
        </div>
        <div className="h-16 w-16 rounded-full bg-gray-100 border border-gray-200 flex items-center justify-center relative group cursor-pointer">
          <span className="text-xl font-medium text-gray-600">
            {profile.firstName.charAt(0)}{profile.lastName.charAt(0)}
          </span>
          <div className="absolute inset-0 bg-black/40 rounded-full flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
            <Camera size={20} className="text-white" />
          </div>
        </div>
      </div>

      <div className="p-6">
        <form onSubmit={handleSubmit((data) => updateMutation.mutateAsync(data))} className="space-y-5">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <div className="space-y-1.5">
              <label className="text-sm font-medium text-gray-700">First Name</label>
              <input {...register("firstName")}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all" />
            </div>
            <div className="space-y-1.5">
              <label className="text-sm font-medium text-gray-700">Last Name</label>
              <input {...register("lastName")}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all" />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium text-gray-700">Email Address</label>
            <div className="relative">
              <input type="email" value={profile.email} disabled
                className="w-full px-3 py-2 bg-gray-50 border border-gray-200 text-gray-500 rounded-lg shadow-sm outline-none cursor-not-allowed" />
              <div className="absolute right-3 top-1/2 -translate-y-1/2 flex items-center gap-1 text-xs font-medium text-green-600 bg-green-50 px-2 py-1 rounded-full">
                <ShieldCheck size={14} />
                Verified
              </div>
            </div>
            <p className="text-xs text-gray-500">Email address cannot be changed directly. Contact support for help.</p>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium text-gray-700">Phone Number</label>
            <input {...register("phone")}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all" />
          </div>

          <div className="pt-4 flex justify-end">
            <button type="submit" disabled={updateMutation.isPending}
              className="px-5 py-2.5 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 focus:ring-4 focus:ring-gray-200 transition-all disabled:opacity-50">
              {updateMutation.isPending ? "Saving..." : "Save Changes"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
