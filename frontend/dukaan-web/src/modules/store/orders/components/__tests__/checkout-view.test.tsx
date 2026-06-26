import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { CheckoutView } from "../checkout-view";
import * as hooks from "../../hooks";

jest.mock("../../hooks", () => ({
  useCustomerAddresses: jest.fn(),
  usePlaceOrder: jest.fn(),
}));

const mockUseCustomerAddresses = hooks.useCustomerAddresses as jest.MockedFunction<
  typeof hooks.useCustomerAddresses
>;
const mockUsePlaceOrder = hooks.usePlaceOrder as jest.MockedFunction<
  typeof hooks.usePlaceOrder
>;

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

describe("CheckoutView", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUsePlaceOrder.mockReturnValue({
      mutate: jest.fn(),
      isPending: false,
    } as unknown as ReturnType<typeof hooks.usePlaceOrder>);
  });

  it("renders loading state", () => {
    mockUseCustomerAddresses.mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
    } as unknown as ReturnType<typeof hooks.useCustomerAddresses>);

    render(<CheckoutView slug="store-1" token="token-123" onBack={jest.fn()} onSuccess={jest.fn()} />, {
      wrapper: createWrapper(),
    });

    expect(screen.getByText("Loading addresses...")).toBeInTheDocument();
  });

  it("renders address selection when addresses loaded", () => {
    mockUseCustomerAddresses.mockReturnValue({
      data: [
        {
          id: "addr-1",
          label: "Home",
          type: "Billing",
          street: "123 Main St",
          city: "Dhaka",
          district: "Dhaka",
          postalCode: "1000",
          phone: "+8801234567890",
          isDefault: true,
        },
      ],
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof hooks.useCustomerAddresses>);

    render(<CheckoutView slug="store-1" token="token-123" onBack={jest.fn()} onSuccess={jest.fn()} />, {
      wrapper: createWrapper(),
    });

    expect(screen.getByText("Billing Address")).toBeInTheDocument();
    expect(screen.getByText("Delivery Address")).toBeInTheDocument();
    expect(screen.getByText("Place Order")).toBeInTheDocument();
  });
});
