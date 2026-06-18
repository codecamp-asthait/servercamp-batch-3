import { render, screen } from "@testing-library/react";
import { MerchantSidebar } from "../merchant-sidebar";

jest.mock("next/navigation", () => ({
  usePathname: () => "/merchant/dashboard",
  useRouter: () => ({ push: jest.fn() }),
}));

describe("MerchantSidebar", () => {
  it("renders the merchant email", () => {
    render(<MerchantSidebar email="test@example.com" />);
    expect(screen.getByText("test@example.com")).toBeInTheDocument();
  });

  it("renders Dashboard and Products nav links", () => {
    render(<MerchantSidebar email="test@example.com" />);
    expect(screen.getByRole("link", { name: /dashboard/i })).toHaveAttribute(
      "href",
      "/merchant/dashboard"
    );
    expect(screen.getByRole("link", { name: /products/i })).toHaveAttribute(
      "href",
      "/merchant/products"
    );
  });

  it("renders a logout button", () => {
    render(<MerchantSidebar email="test@example.com" />);
    expect(screen.getByRole("button", { name: /logout/i })).toBeInTheDocument();
  });

  it("highlights the active nav link", () => {
    render(<MerchantSidebar email="test@example.com" />);
    const dashboardLink = screen.getByRole("link", { name: /dashboard/i });
    expect(dashboardLink).toHaveClass("font-semibold");
  });
});
