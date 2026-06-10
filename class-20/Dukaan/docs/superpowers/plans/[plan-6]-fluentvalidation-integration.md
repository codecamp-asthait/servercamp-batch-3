# [plan-6] FluentValidation Filter Integration Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Integrate FluentValidation using a custom Action Filter to validate API request models.

**Architecture:** Create a custom `ActionFilter` in `dukaan.Host` that resolves `IValidator<T>` for the action arguments. Register validators in `Program.cs`.

**Tech Stack:** FluentValidation

---

### Task 1: Add NuGet Packages

**Files:**
- Modify: `dukaan.Host/dukaan.Host.csproj`

- [ ] **Step 1: Add FluentValidation package**

Run:
```bash
dotnet add dukaan.Host/dukaan.Host.csproj package FluentValidation
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Host/dukaan.Host.csproj
git commit -m "feat: add fluentvalidation nuget package"
```

### Task 2: Implement Validation Filter

**Files:**
- Create: `dukaan.Host/Filters/ValidationFilter.cs`

- [ ] **Step 1: Create ValidationFilter**

```csharp
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dukaan.Host.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationResult = validator.Validate(new ValidationContext<object>(argument));
                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Host/Filters/ValidationFilter.cs
git commit -m "feat: add validation action filter"
```

### Task 3: Register Services in Program.cs

**Files:**
- Modify: `dukaan.Host/Program.cs`

- [ ] **Step 1: Register Filter and Validators**

Update `Program.cs`:

```csharp
using FluentValidation;
using dukaan.Host.Filters;

// ...
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Register validators from the Application assembly
builder.Services.AddValidatorsFromAssembly(typeof(dukaan.Application.Services.ITenantService).Assembly);
```

- [ ] **Step 2: Run build to verify no errors**

Run: `dotnet build dukaan.Host`
Expected: SUCCESS

- [ ] **Step 3: Commit**

```bash
git add dukaan.Host/Program.cs
git commit -m "feat: configure validation filter and register validators"
```
