# CodeCamp ServerCamp Batch 3 - Class 5

This repository contains two projects focused on understanding the inner workings of web frameworks and middleware in .NET.

## Projects

### 1. [learning-middleware](./learning-middleware)
A hands-on exploration of ASP.NET Core Middleware. It demonstrates how request and response pipelines work in a real-world ASP.NET Core application.

**Key Features:**
- **Custom Middleware:** Implementation of `GlobalErrorHandlingMiddleware` to centralize exception handling.
- **Inline Middleware:** Examples of using `app.Use(...)` to intercept requests and responses.
- **Middleware Order:** Demonstrates the "onion" model of middleware execution (Before `next()` and After `next()`).
- **Error Handling:** Shows how to catch and format errors gracefully.

### 2. [web-app-scratch](./web-app-scratch)
A "from-scratch" implementation of a web framework and dependency injection container. This project avoids using the built-in ASP.NET Core libraries to show how things work under the hood using TCP sockets and reflection.

**Key Components:**
- **TCP Server:** A basic HTTP server built using `TcpListener` that parses raw HTTP requests and sends responses.
- **Routing System:** A custom `Router` that maps HTTP methods and paths to specific handlers or controller actions.
- **Dependency Injection (DI) Container:** A custom implementation of `ServiceCollection` and `ServiceProvider` supporting `Transient`, `Scoped`, and `Singleton` lifetimes.
- **Controller Discovery:** Uses Reflection to automatically find classes ending in `Controller` and map their methods marked with `HttpGet` or `HttpPost` attributes to routes.
- **Mini WebApplication:** A facade that mimics the modern `WebApplicationBuilder` and `WebApplication` API found in ASP.NET Core.

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

### Running the Projects

#### Learning Middleware
```bash
cd learning-middleware
dotnet run
```
The application will start (usually on `http://localhost:5000` or `5001`). 
- Visit `/` for a "Hello World" response.
- Visit `/error` to see the global error handler in action.

#### Web App from Scratch
```bash
cd web-app-scratch
dotnet run
```
The custom server starts on port `5005`.
- Visit `http://localhost:5005/codecamp` to see route handling.
- Visit `http://localhost:5005/user` (if defined in `UserController`) to see controller-based routing and DI in action.

## Learning Objectives
- Understand the HTTP Request/Response lifecycle.
- Learn how Middleware intercepts and processes requests.
- Deep dive into how Dependency Injection containers manage object lifetimes.
- Explore how Reflection allows for "magic" features like automatic controller discovery and routing.
- Grasp the basics of socket programming and HTTP protocol parsing.
