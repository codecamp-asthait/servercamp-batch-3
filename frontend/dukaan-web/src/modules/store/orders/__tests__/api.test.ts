import { ordersApi } from "../api";
import { http } from "@/lib/http";

jest.mock("@/lib/http");

describe("ordersApi", () => {
  const mockHttp = http as jest.MockedFunction<typeof http>;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("placeOrder calls POST /api/orders with correct params", async () => {
    const mockOrder = { id: "order-1", orderNumber: "ORD-000001" };
    mockHttp.mockResolvedValue(mockOrder);

    const result = await ordersApi.placeOrder("store-1", "token-123", {
      billingAddressId: "addr-1",
      deliveryAddressId: "addr-2",
    });

    expect(mockHttp).toHaveBeenCalledWith("/api/orders", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "x-tenant-slug": "store-1",
        Authorization: "Bearer token-123",
      },
      body: JSON.stringify({
        billingAddressId: "addr-1",
        deliveryAddressId: "addr-2",
      }),
    });
    expect(result).toEqual(mockOrder);
  });

  it("getAddresses calls GET /api/addresses", async () => {
    const mockAddresses = [{ id: "addr-1", label: "Home" }];
    mockHttp.mockResolvedValue(mockAddresses);

    const result = await ordersApi.getAddresses("store-1", "token-123");

    expect(mockHttp).toHaveBeenCalledWith("/api/addresses", {
      headers: {
        "Content-Type": "application/json",
        "x-tenant-slug": "store-1",
        Authorization: "Bearer token-123",
      },
    });
    expect(result).toEqual(mockAddresses);
  });

  it("getOrders calls GET /api/orders with pagination", async () => {
    const mockResponse = { items: [], totalCount: 0, pageNumber: 1, pageSize: 10 };
    mockHttp.mockResolvedValue(mockResponse);

    const result = await ordersApi.getOrders("store-1", "token-123", 2);

    expect(mockHttp).toHaveBeenCalledWith("/api/orders?pageNumber=2&pageSize=10", {
      headers: {
        "Content-Type": "application/json",
        "x-tenant-slug": "store-1",
        Authorization: "Bearer token-123",
      },
    });
    expect(result).toEqual(mockResponse);
  });

  it("getOrder calls GET /api/orders/{id}", async () => {
    const mockOrder = { id: "order-1" };
    mockHttp.mockResolvedValue(mockOrder);

    const result = await ordersApi.getOrder("store-1", "token-123", "order-1");

    expect(mockHttp).toHaveBeenCalledWith("/api/orders/order-1", {
      headers: {
        "Content-Type": "application/json",
        "x-tenant-slug": "store-1",
        Authorization: "Bearer token-123",
      },
    });
    expect(result).toEqual(mockOrder);
  });
});
