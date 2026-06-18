using Hangfire.Dashboard;

namespace Dukaan.Host;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
