import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { OrderDetailView } from "../order-detail-view";
import * as hooks from "../../hooks";

jest.mock("../../hooks", () => ({
  useOrder: jest.fn(),
}));

jest.mock("@/lib/local-storage.service", () => ({
  localStorageService: {
    getCustomerToken: jest.fn().mockReturnValue("token-123"),
  },
}));

jest.mock("next/navigation", () => ({
  useRouter: jest.fn().mockReturnValue({ push: jest.fn() }),
}));

const mockUseOrder = hooks.useOrder as jest.MockedFunction<typeof hooks.useOrder>;

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  const Wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
  Wrapper.displayName = "QueryClientWrapper";
  return Wrapper;
};

describe("OrderDetailView", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders order details when order exists", () => {
    mockUseOrder.mockReturnValue({
      data: {
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
        items: [
          {
            productId: "prod-1",
            productName: "Test Product",
            unitPrice: 50,
            quantity: 2,
            subtotal: 100,
          },
        ],
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof hooks.useOrder>);

    render(<OrderDetailView slug="store-1" orderId="order-1" />, { wrapper: createWrapper() });

    expect(screen.getByText("ORD-000001")).toBeInTheDocument();
    expect(screen.getByText("Test Product")).toBeInTheDocument();
  });

  it("renders not found when order is 404", () => {
    mockUseOrder.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
    } as unknown as ReturnType<typeof hooks.useOrder>);

    render(<OrderDetailView slug="store-1" orderId="invalid" />, { wrapper: createWrapper() });

    expect(screen.getByText("Order not found")).toBeInTheDocument();
  });
});
