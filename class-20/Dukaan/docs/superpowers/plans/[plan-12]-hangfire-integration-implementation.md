# Hangfire Integration Implementation Plan

Status: Pending
Date: 2026-05-07

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Integrate Hangfire into Dukaan for background processing with full multi-tenant context awareness and a secured dashboard.

**Architecture:** Use `Hangfire.PostgreSql` for storage. Implement a custom Job Filter to flow `TenantId` via job metadata. Secure the dashboard using a Basic Auth filter for production environments.

**Tech Stack:** .NET 8, Hangfire, PostgreSQL, EF Core.

---

### Task 1: Install Dependencies

**Files:**
- Modify: `dukaan.Infrastructure/dukaan.Infrastructure.csproj`
- Modify: `dukaan.Host/dukaan.Host.csproj`

- [ ] **Step 1: Add Hangfire.PostgreSql to Infrastructure**
Add `<PackageReference Include="Hangfire.PostgreSql" Version="1.20.9" />` to `dukaan.Infrastructure.csproj`.

- [ ] **Step 2: Add Hangfire.AspNetCore to Host**
Add `<PackageReference Include="Hangfire.AspNetCore" Version="1.8.11" />` to `dukaan.Host.csproj`.

- [ ] **Step 3: Restore packages**
Run: `dotnet restore`

- [ ] **Step 4: Commit**
```bash
git add dukaan.Infrastructure/dukaan.Infrastructure.csproj dukaan.Host/dukaan.Host.csproj
git commit -m "chore: add hangfire dependencies"
```

### Task 2: Implement TenantJobFilter

**Files:**
- Create: `dukaan.Infrastructure/Filters/TenantJobFilter.cs`

- [ ] **Step 1: Create the TenantJobFilter class**
```csharp
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using dukaan.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace dukaan.Infrastructure.Filters;

public class TenantJobFilter(IServiceProvider serviceProvider) : IClientFilter, IServerFilter
{
    private const string TenantIdKey = "TenantId";

    public void OnCreating(CreatingContext filterContext)
    {
        using var scope = serviceProvider.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        var tenantId = tenantProvider.GetTenantId();

        if (tenantId.HasValue)
        {
            filterContext.SetJobParameter(TenantIdKey, tenantId.Value.ToString());
        }
    }

    public void OnCreated(CreatedContext filterContext) {}

    public void OnPerforming(PerformingContext filterContext)
    {
        var tenantIdString = filterContext.GetJobParameter<string>(TenantIdKey);
        if (!string.IsNullOrEmpty(tenantIdString) && Guid.TryParse(tenantIdString, out var tenantId))
        {
            var tenantProvider = filterContext.Items.GetOrDefault("TenantProvider") as ITenantProvider 
                                ?? filterContext.Scope.ServiceProvider.GetRequiredService<ITenantProvider>();
            
            tenantProvider.SetTenantId(tenantId);
        }
    }

    public void OnPerformed(PerformedContext filterContext) {}
}
```

- [ ] **Step 2: Commit**
```bash
git add dukaan.Infrastructure/Filters/TenantJobFilter.cs
git commit -m "feat: add TenantJobFilter for multi-tenant context flow"
```

### Task 3: Implement Dashboard Authorization

**Files:**
- Create: `dukaan.Infrastructure/Filters/HangfireDashboardAuthorizationFilter.cs`

- [ ] **Step 1: Create the Authorization Filter**
```csharp
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace dukaan.Infrastructure.Filters;

public class HangfireDashboardAuthorizationFilter(IConfiguration configuration) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow local access without auth in Development
        if (httpContext.Request.Host.Host == "localhost" || httpContext.Request.Host.Host == "127.0.0.1")
        {
            return true;
        }

        var header = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(header))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        var authHeader = AuthenticationHeaderValue.Parse(header);
        if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        var parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter!));
        var parts = parameter.Split(':');

        if (parts.Length != 2) return false;

        var user = parts[0];
        var pass = parts[1];

        var configUser = configuration["Hangfire:User"];
        var configPass = configuration["Hangfire:Password"];

        if (user == configUser && pass == configPass)
        {
            return true;
        }

        SetChallengeResponse(httpContext);
        return false;
    }

    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}
```

- [ ] **Step 2: Commit**
```bash
git add dukaan.Infrastructure/Filters/HangfireDashboardAuthorizationFilter.cs
git commit -m "feat: add HangfireDashboardAuthorizationFilter with Basic Auth support"
```

### Task 4: Configure Hangfire in Infrastructure

**Files:**
- Create: `dukaan.Infrastructure/Extensions/HangfireExtensions.cs`

- [ ] **Step 1: Create HangfireExtensions class**
```csharp
using Hangfire;
using Hangfire.PostgreSql;
using dukaan.Infrastructure.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dukaan.Infrastructure.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddDukaanHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => 
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            .UseFilter(new TenantJobFilter(services.BuildServiceProvider())));

        services.AddHangfireServer();

        return services;
    }
}
```

- [ ] **Step 2: Commit**
```bash
git add dukaan.Infrastructure/Extensions/HangfireExtensions.cs
git commit -m "feat: add Hangfire service registration extensions"
```

### Task 5: Register and Map in Host

**Files:**
- Modify: `dukaan.Host/Program.cs`

- [ ] **Step 1: Register Hangfire services**
Add `builder.Services.AddDukaanHangfire(builder.Configuration);` before `builder.Build();`.

- [ ] **Step 2: Map Hangfire Dashboard**
Add the dashboard mapping after `app.UseAuthorization();`:
```csharp
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter(app.Configuration) }
});
```

- [ ] **Step 3: Commit**
```bash
git add dukaan.Host/Program.cs
git commit -m "feat: enable Hangfire and Dashboard in Host"
```

### Task 6: Verification and Test Job

**Files:**
- Create: `dukaan.Application/Services/IBackgroundJobService.cs`
- Create: `dukaan.Application/Services/BackgroundJobService.cs`
- Create: `dukaan.Host/Controllers/JobsController.cs`

- [ ] **Step 1: Create BackgroundJobService**
```csharp
using Microsoft.Extensions.Logging;
using dukaan.Domain.Interfaces;

namespace dukaan.Application.Services;

public interface IBackgroundJobService
{
    void ProcessTestJob(string message);
}

public class BackgroundJobService(ILogger<BackgroundJobService> logger, ITenantProvider tenantProvider) : IBackgroundJobService
{
    public void ProcessTestJob(string message)
    {
        var tenantId = tenantProvider.GetTenantId();
        logger.LogInformation("Processing background job for Tenant: {TenantId}. Message: {Message}", tenantId, message);
    }
}
```

- [ ] **Step 2: Register the service in Program.cs**
Add `builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();`.

- [ ] **Step 3: Create JobsController**
```csharp
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using dukaan.Application.Services;

namespace dukaan.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    [HttpPost("test")]
    public IActionResult EnqueueTestJob([FromQuery] string message)
    {
        BackgroundJob.Enqueue<IBackgroundJobService>(x => x.ProcessTestJob(message));
        return Ok(new { Message = "Job enqueued successfully" });
    }
}
```

- [ ] **Step 4: Commit**
```bash
git add dukaan.Application/Services/IBackgroundJobService.cs dukaan.Application/Services/BackgroundJobService.cs dukaan.Host/Controllers/JobsController.cs dukaan.Host/Program.cs
git commit -m "feat: add verification job and controller"
```

### Task 7: Final Verification

- [ ] **Step 1: Run the application**
Run: `dotnet run --project dukaan.Host`

- [ ] **Step 2: Access Dashboard**
Visit `http://localhost:<port>/hangfire`. Verify it opens without credentials on localhost.

- [ ] **Step 3: Trigger Job**
Use Postman or Swagger to call `POST /api/jobs/test?message=HelloHangfire`. 
Ensure you pass a `X-Tenant-ID` header if required by your current middleware.

- [ ] **Step 4: Verify Logs**
Check console output for: `Processing background job for Tenant: <ID>. Message: HelloHangfire`.
Verify that the `TenantId` matches the one passed in the request.
