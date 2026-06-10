# [plan-3] Serilog Development Setup Implementation Plan

Status: Completed

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Configure Serilog to log to the console, specifically in the development environment.

**Architecture:** Use `Serilog.AspNetCore` in the `dukaan.Host` project. Configure Serilog during host building, conditionally checking for the Development environment.

**Tech Stack:** Serilog, Serilog.AspNetCore, Serilog.Sinks.Console

---

### Task 1: Add NuGet Packages

**Files:**
- Modify: `dukaan.Host/dukaan.Host.csproj`

- [ ] **Step 1: Add required NuGet packages**

Run:
```bash
dotnet add dukaan.Host/dukaan.Host.csproj package Serilog.AspNetCore
dotnet add dukaan.Host/dukaan.Host.csproj package Serilog.Sinks.Console
```

- [ ] **Step 2: Commit**

```bash
git add dukaan.Host/dukaan.Host.csproj
git commit -m "feat: add serilog nuget packages"
```

### Task 2: Configure Serilog in Program.cs

**Files:**
- Modify: `dukaan.Host/Program.cs`

- [ ] **Step 1: Update Program.cs to configure Serilog for Development**

Replace the builder setup in `Program.cs`:

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseSerilog((context, configuration) => configuration
        .WriteTo.Console()
        .MinimumLevel.Information());
}

// ... rest of builder configuration
```

- [ ] **Step 2: Run build to verify no errors**

Run: `dotnet build dukaan.Host`
Expected: SUCCESS

- [ ] **Step 3: Commit**

```bash
git add dukaan.Host/Program.cs
git commit -m "feat: configure serilog for development environment"
```
