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
      items: [{ id: "order-1", orderNumber: "ORD-000001" }],
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
    const mockOrder = { id: "order-1", orderNumber: "ORD-000001" };
    mockOrdersApi.getOrder.mockResolvedValue(mockOrder);

    const { result } = renderHook(
      () => useOrder("store-1", "token-123", "order-1"),
      { wrapper: createWrapper() }
    );

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual(mockOrder);
  });

  it("useCustomerAddresses fetches addresses", async () => {
    const mockAddresses = [{ id: "addr-1", label: "Home" }];
    mockOrdersApi.getAddresses.mockResolvedValue(mockAddresses);

    const { result } = renderHook(() => useCustomerAddresses("store-1", "token-123"), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual(mockAddresses);
  });
});
