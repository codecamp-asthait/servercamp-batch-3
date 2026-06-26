import { renderHook, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { usePlaceOrder, useOrders, useOrder, useCustomerAddresses } from "../hooks";
import { ordersApi } from "../api";

jest.mock("../api");

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

describe("orders hooks", () => {
  const mockOrdersApi = ordersApi as jest.Mocked<typeof ordersApi>;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("usePlaceOrder returns a mutation", () => {
    const { result } = renderHook(() => usePlaceOrder("store-1", "token-123"), {
      wrapper: createWrapper(),
    });
    expect(result.current.mutate).toBeDefined();
  });

  it("useOrders fetches orders list", async () => {
    const mockOrders = {
      items: [{
        id: "order-1",
        orderNumber: "ORD-000001",
        status: "Pending" as const,
        total: 100,
        itemCount: 2,
        createdAt: "2026-06-27T10:00:00Z",
      }],
      totalCount: 1,
      pageNumber: 1,
      pageSize: 10,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    };
    mockOrdersApi.getOrders.mockResolvedValue(mockOrders);

    const { result } = renderHook(() => useOrders("store-1", "token-123", 1), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual(mockOrders);
  });

  it("useOrder fetches single order", async () => {
    const mockOrder = {
      id: "order-1",
      orderNumber: "ORD-000001",
      status: "Pending" as const,
      billingAddress: {
        street: "123 Main St",
        city: "Dhaka",
        district: "Dhaka",
        postalCode: "1205",
        phone: "+8801700000000",
      },
      deliveryAddress: {
        street: "123 Main St",
        city: "Dhaka",
        district: "Dhaka",
        postalCode: "1205",
        phone: "+8801700000000",
      },
      subtotal: 100,
      total: 100,
      createdAt: "2026-06-27T10:00:00Z",
      items: [{
        productId: "prod-1",
        productName: "Product 1",
        unitPrice: 50,
        quantity: 2,
        subtotal: 100,
      }],
    };
    mockOrdersApi.getOrder.mockResolvedValue(mockOrder);

    const { result } = renderHook(
      () => useOrder("store-1", "token-123", "order-1"),
      { wrapper: createWrapper() }
    );

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual(mockOrder);
  });

  it("useCustomerAddresses fetches addresses", async () => {
    const mockAddresses = [{
      id: "addr-1",
      label: "Home",
      type: "Billing" as const,
      street: "123 Main St",
      city: "Dhaka",
      district: "Dhaka",
      postalCode: "1205",
      phone: "+8801700000000",
      isDefault: true,
    }];
    mockOrdersApi.getAddresses.mockResolvedValue(mockAddresses);

    const { result } = renderHook(() => useCustomerAddresses("store-1", "token-123"), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual(mockAddresses);
  });
});
