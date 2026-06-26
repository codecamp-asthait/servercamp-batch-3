import { render, screen } from "@testing-library/react";
import { DashboardView } from "../dashboard-view";

describe("DashboardView", () => {
  it("renders the dashboard heading", () => {
    render(<DashboardView />);
    expect(
      screen.getByRole("heading", { name: /dashboard/i })
    ).toBeInTheDocument();
  });
});
