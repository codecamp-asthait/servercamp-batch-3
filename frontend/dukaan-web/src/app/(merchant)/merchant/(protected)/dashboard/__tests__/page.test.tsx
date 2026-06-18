import { render, screen } from "@testing-library/react";

jest.mock(
  "@/modules/merchant/dashboard/components/dashboard-view",
  () => ({
    DashboardView: ({ email }: { email: string }) => (
      <h1>Welcome back, {email}</h1>
    ),
  })
);

it("page default export renders DashboardView", async () => {
  const Page = (await import("../page")).default;
  render(<Page />);
  expect(screen.getByRole("heading")).toBeInTheDocument();
});
