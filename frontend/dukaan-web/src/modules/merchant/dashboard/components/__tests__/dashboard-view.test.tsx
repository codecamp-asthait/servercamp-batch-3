import { render, screen } from "@testing-library/react";
import { DashboardView } from "../dashboard-view";

describe("DashboardView", () => {
  it("renders a welcome heading with the merchant email", () => {
    render(<DashboardView email="merchant@store.com" />);
    expect(
      screen.getByRole("heading", { name: /welcome back, merchant@store\.com/i })
    ).toBeInTheDocument();
  });
});
