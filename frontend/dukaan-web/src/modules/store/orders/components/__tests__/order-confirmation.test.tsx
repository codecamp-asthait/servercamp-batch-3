import { render, screen } from "@testing-library/react";
import { OrderConfirmation } from "../order-confirmation";

describe("OrderConfirmation", () => {
  it("renders order number and success message", () => {
    render(
      <OrderConfirmation
        order={{
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
        }}
        slug="store-1"
      />
    );

    expect(screen.getByText("Order Placed!")).toBeInTheDocument();
    expect(screen.getByText("ORD-000001")).toBeInTheDocument();
  });
});
