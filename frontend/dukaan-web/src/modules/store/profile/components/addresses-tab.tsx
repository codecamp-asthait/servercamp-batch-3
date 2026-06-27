"use client";

import { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { MapPin } from "lucide-react";
import { useCustomerAddresses, useCreateAddress, useUpdateAddress, useDeleteAddress, useSetDefaultAddress } from "@/modules/store/orders/hooks";
import type { AddressDto, CreateAddressData } from "@/modules/store/orders/types";

interface AddressesTabProps {
  slug: string;
  token: string | null;
}

export function AddressesTab({ slug, token }: AddressesTabProps) {
  const { data: addresses, isLoading } = useCustomerAddresses(slug, token ?? "");
  const createMutation = useCreateAddress(slug, token ?? "");
  const updateMutation = useUpdateAddress(slug, token ?? "");
  const deleteMutation = useDeleteAddress(slug, token ?? "");
  const setDefaultMutation = useSetDefaultAddress(slug, token ?? "");
  const { register, handleSubmit, reset, setValue } = useForm<CreateAddressData>();
  const editIdRef = useRef<string | null>(null);
  const [showForm, setShowForm] = useState(false);

  const closeForm = () => { setShowForm(false); editIdRef.current = null; reset(); };

  const startEdit = (addr: AddressDto) => {
    editIdRef.current = addr.id;
    setValue("label", addr.label);
    setValue("type", addr.type);
    setValue("street", addr.street);
    setValue("city", addr.city);
    setValue("district", addr.district);
    setValue("postalCode", addr.postalCode);
    setValue("phone", addr.phone);
    setValue("isDefault", addr.isDefault);
    setShowForm(true);
  };

  const onSubmit = async (data: CreateAddressData) => {
    if (editIdRef.current) await updateMutation.mutateAsync({ id: editIdRef.current, data });
    else await createMutation.mutateAsync(data);
    closeForm();
  };

  if (isLoading) return <div className="text-sm text-gray-500">Loading...</div>;

  if (!addresses || addresses.length === 0) {
    return (
      <div className="w-full flex items-center justify-center min-h-[300px]">
        <div className="text-center">
          <div className="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4 text-gray-400">
            <MapPin size={24} />
          </div>
          <h3 className="text-lg font-medium text-gray-900 mb-1">No addresses saved</h3>
          <p className="text-gray-500 text-sm mb-6">You haven't added any shipping addresses yet.</p>
          <button
            onClick={() => { editIdRef.current = null; reset(); setShowForm(true); }}
            className="px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 transition-colors"
          >
            Add New Address
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <button
          onClick={() => { editIdRef.current = null; reset(); setShowForm(true); }}
          className="px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 transition-colors"
        >
          Add Address
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleSubmit(onSubmit)} className="w-full bg-white rounded-xl border border-gray-200 shadow-sm p-6 space-y-4">
          <h4 className="text-base font-semibold text-gray-900">{editIdRef.current ? "Edit Address" : "New Address"}</h4>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <input {...register("label")} placeholder="Label (e.g. Home, Office)"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
            <input {...register("phone")} placeholder="Phone"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
          </div>
          <input {...register("street")} placeholder="Street"
            className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <input {...register("city")} placeholder="City"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
            <input {...register("district")} placeholder="District"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
            <input {...register("postalCode")} placeholder="Postal Code"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm" />
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <select {...register("type")}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:ring-2 focus:ring-gray-900 focus:border-gray-900 outline-none transition-all text-sm bg-white">
              <option value="Delivery">Delivery</option>
              <option value="Billing">Billing</option>
            </select>
            <label className="flex items-center gap-2 text-sm text-gray-700">
              <input type="checkbox" {...register("isDefault")} className="rounded" />
              Set as default
            </label>
          </div>
          <div className="flex gap-3 pt-2">
            <button type="submit"
              className="px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 transition-colors">Save</button>
            <button type="button" onClick={closeForm}
              className="px-4 py-2 border border-gray-300 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition-colors">Cancel</button>
          </div>
        </form>
      )}

      {addresses.map((addr) => (
        <div key={addr.id} className="w-full bg-white rounded-xl border border-gray-200 shadow-sm p-5 flex items-start justify-between">
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <span className="text-sm font-semibold text-gray-900">{addr.label}</span>
              {addr.isDefault && <span className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded-full font-medium">Default</span>}
            </div>
            <p className="text-sm text-gray-500">{addr.street}, {addr.city}, {addr.district} {addr.postalCode}</p>
            <p className="text-sm text-gray-500">{addr.phone} &middot; {addr.type}</p>
          </div>
          <div className="flex gap-2 shrink-0">
            <button onClick={() => startEdit(addr)}
              className="text-sm text-gray-600 hover:text-gray-900 font-medium px-2 py-1 rounded hover:bg-gray-100 transition-colors">Edit</button>
            {!addr.isDefault && (
              <button onClick={() => setDefaultMutation.mutate(addr.id)}
                className="text-sm text-gray-600 hover:text-gray-900 font-medium px-2 py-1 rounded hover:bg-gray-100 transition-colors">Set Default</button>
            )}
            <button onClick={() => deleteMutation.mutate(addr.id)}
              className="text-sm text-red-600 hover:text-red-700 font-medium px-2 py-1 rounded hover:bg-red-50 transition-colors">Delete</button>
          </div>
        </div>
      ))}
    </div>
  );
}
