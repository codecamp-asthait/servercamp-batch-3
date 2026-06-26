import { render, screen } from "@testing-library/react";
import MerchantLayout from "../(protected)/layout";

jest.mock("next/navigation", () => ({
  usePathname: () => "/merchant/dashboard",
  useRouter: () => ({ push: jest.fn() }),
}));

// Payload: { "email": "owner@shop.com" }
const FAKE_JWT =
  "eyJhbGciOiJIUzI1NiJ9." +
  btoa(JSON.stringify({ email: "owner@shop.com" })) +
  ".sig";

beforeEach(() => {
  Object.defineProperty(document, "cookie", {
    writable: true,
    value: `token=${FAKE_JWT}`,
  });
});

describe("MerchantLayout", () => {
  it("renders the sidebar and children", () => {
    render(
      <MerchantLayout>
        <div>page content</div>
      </MerchantLayout>
    );
    expect(screen.getByText("owner@shop.com")).toBeInTheDocument();
    expect(screen.getByText("page content")).toBeInTheDocument();
  });
});
