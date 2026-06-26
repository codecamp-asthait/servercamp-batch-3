"use client";

import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { HttpError } from "@/lib/http";
import { useCustomerAddresses, usePlaceOrder } from "../hooks";
import type { AddressDto } from "../types";

interface Props {
  slug: string;
  token: string;
  onBack: () => void;
  onSuccess: (orderId: string) => void;
}

export function CheckoutView({ slug, token, onBack, onSuccess }: Props) {
  const { data: addresses, isLoading, isError } = useCustomerAddresses(slug, token);
  const { mutate: placeOrder, isPending } = usePlaceOrder(slug, token);
  const billingAddresses = addresses?.filter((a) => a.type === "Billing") ?? [];

  const deliveryAddresses = addresses?.filter((a) => a.type === "Delivery") ?? [];

  const defaultBilling = billingAddresses.find((a) => a.isDefault);
  const defaultDelivery = deliveryAddresses.find((a) => a.isDefault);

  const [billingAddressId, setBillingAddressId] = useState(defaultBilling?.id ?? "");
  const [deliveryAddressId, setDeliveryAddressId] = useState(defaultDelivery?.id ?? "");
  const [sameAsBilling, setSameAsBilling] = useState(false);

  const handlePlaceOrder = () => {
    if (!billingAddressId) {
      toast.error("Please select a billing address");
      return;
    }
    const finalDeliveryId = sameAsBilling ? billingAddressId : deliveryAddressId;
    if (!finalDeliveryId) {
      toast.error("Please select a delivery address");
      return;
    }

    placeOrder(
      { billingAddressId, deliveryAddressId: finalDeliveryId },
      {
        onSuccess: (order) => {
          onSuccess(order.id);
        },
        onError: (error: Error) => {
          const httpError = error as HttpError;
          if (httpError.status === 400) {
            toast.error("Your cart is empty or products are unavailable");
          } else if (httpError.status === 404) {
            toast.error("Selected address no longer exists");
          } else {
            toast.error("Something went wrong, please try again");
          }
        },
      }
    );
  };

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-16">
        <p className="text-sm text-zinc-500">Loading addresses...</p>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex flex-col items-center justify-center gap-4 py-16 px-4">
        <p className="text-sm text-destructive">Failed to load addresses</p>
        <Button variant="outline" size="sm" onClick={onBack}>
          Back to Cart
        </Button>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4 px-4 py-4">
      <div>
        <h3 className="text-sm font-semibold mb-2">Billing Address</h3>
        <div className="flex flex-col gap-2">
          {billingAddresses.map((addr) => (
            <AddressRadio
              key={addr.id}
              address={addr}
              selected={billingAddressId === addr.id}
              onChange={() => setBillingAddressId(addr.id)}
            />
          ))}
        </div>
      </div>

      <Separator />

      <div>
        <label className="flex items-center gap-2 mb-2">
          <input
            type="checkbox"
            checked={sameAsBilling}
            onChange={(e) => {
              setSameAsBilling(e.target.checked);
              if (e.target.checked) {
                setDeliveryAddressId(billingAddressId);
              }
            }}
            className="h-4 w-4"
          />
          <span className="text-sm">Same as billing</span>
        </label>

        {!sameAsBilling && (
          <div className="flex flex-col gap-2">
            <h3 className="text-sm font-semibold mb-2">Delivery Address</h3>
            {deliveryAddresses.map((addr) => (
              <AddressRadio
                key={addr.id}
                address={addr}
                selected={deliveryAddressId === addr.id}
                onChange={() => setDeliveryAddressId(addr.id)}
              />
            ))}
          </div>
        )}
      </div>

      <Separator />

      <div className="flex flex-col gap-2">
        <Button onClick={handlePlaceOrder} disabled={isPending || !billingAddressId}>
          {isPending ? "Placing Order..." : "Place Order"}
        </Button>
        <Button variant="outline" onClick={onBack}>
          Back to Cart
        </Button>
      </div>
    </div>
  );
}

function AddressRadio({
  address,
  selected,
  onChange,
}: {
  address: AddressDto;
  selected: boolean;
  onChange: () => void;
}) {
  return (
    <label
      className={`flex items-start gap-3 p-3 rounded-lg border cursor-pointer transition-colors ${
        selected ? "border-primary bg-primary/5" : "border-zinc-200 hover:border-zinc-300"
      }`}
    >
      <input
        type="radio"
        checked={selected}
        onChange={onChange}
        className="mt-1"
      />
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium">{address.label}</p>
        <p className="text-xs text-zinc-500 mt-0.5">
          {address.street}, {address.city}, {address.district} {address.postalCode}
        </p>
        <p className="text-xs text-zinc-500">{address.phone}</p>
      </div>
    </label>
  );
}
