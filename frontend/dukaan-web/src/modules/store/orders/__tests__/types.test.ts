import type { OrderDto, AddressDto } from "../types";

describe("Order types", () => {
  it("OrderDto has correct shape", () => {
    const order: OrderDto = {
      id: "order-1",
      orderNumber: "ORD-000001",
      status: "Pending",
      billingAddress: {
        street: "123 Main St",
        city: "Dhaka",
        district: "Dhaka",
        postalCode: "1000",
        phone: "+8801234567890",
      },
      deliveryAddress: {
        street: "123 Main St",
        city: "Dhaka",
        district: "Dhaka",
        postalCode: "1000",
        phone: "+8801234567890",
      },
      subtotal: 100,
      total: 100,
      createdAt: "2026-06-22T10:00:00Z",
      items: [],
    };
    expect(order.id).toBe("order-1");
  });

  it("AddressDto has correct shape", () => {
    const address: AddressDto = {
      id: "addr-1",
      label: "Home",
      type: "Billing",
      street: "123 Main St",
      city: "Dhaka",
      district: "Dhaka",
      postalCode: "1000",
      phone: "+8801234567890",
      isDefault: true,
    };
    expect(address.id).toBe("addr-1");
  });
});
