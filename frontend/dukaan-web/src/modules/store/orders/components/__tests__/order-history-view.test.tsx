import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { OrderHistoryView } from "../order-history-view";
import * as hooks from "../../hooks";

jest.mock("../../hooks", () => ({
  useOrders: jest.fn(),
}));

jest.mock("@/lib/local-storage.service", () => ({
  localStorageService: {
    getCustomerToken: jest.fn().mockReturnValue("token-123"),
  },
}));

jest.mock("next/navigation", () => ({
  useRouter: jest.fn().mockReturnValue({ push: jest.fn() }),
}));

const mockUseOrders = hooks.useOrders as jest.MockedFunction<typeof hooks.useOrders>;

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

describe("OrderHistoryView", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders empty state when no orders", () => {
    mockUseOrders.mockReturnValue({
      data: {
        items: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof hooks.useOrders>);

    render(<OrderHistoryView slug="store-1" />, { wrapper: createWrapper() });

    expect(screen.getByText("No orders yet")).toBeInTheDocument();
  });

  it("renders order list when orders exist", () => {
    mockUseOrders.mockReturnValue({
      data: {
        items: [
          {
            id: "order-1",
            orderNumber: "ORD-000001",
            status: "Pending",
            total: 100,
            itemCount: 2,
            createdAt: "2026-06-22T10:00:00Z",
          },
        ],
        totalCount: 1,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof hooks.useOrders>);

    render(<OrderHistoryView slug="store-1" />, { wrapper: createWrapper() });

    expect(screen.getByText("ORD-000001")).toBeInTheDocument();
    expect(screen.getByText("Pending")).toBeInTheDocument();
  });
});
