/*
Dependency Inversion Principle (DIP):
- High-level modules should NOT depend on low-level modules
- Both should depend on abstractions (interfaces)

✔ High-Level Module:
   - The caller (business logic)
   - Example: PaymentService

✔ Low-Level Module:
   - The actual implementation
   - Example: EmailService

✔ Abstraction:
   - Interface that connects both
   - Example: INotificationService

Inversion of Control (IoC):
- Dependencies are NOT created inside classes
- They are provided by an external container (DI Container)

Without IoC:
   var service = new PaymentService(new EmailService());

With IoC:
   var service = provider.GetRequiredService<PaymentService>();

Service Lifetimes in Dependency Injection:

1. Singleton
   - Created once for the entire application lifetime
   - Same instance is returned every time

2. Transient
   - New instance is created every time it is requested

3. Scoped
   - One instance per scope (e.g., per HTTP request in ASP.NET)
   - Same instance within a scope, different across scopes

Quick Summary:
DIP → Structure your dependencies (use abstraction)
IoC → Who controls object creation (container)
DI  → How dependencies are injected (implementation)
*/

using Microsoft.Extensions.DependencyInjection;

// Register services with different lifetimes
var services1 = new ServiceCollection();
services1.AddSingleton<ITestSingletonService, TestSingletonService>();
services1.AddTransient<ITestTransientService, TestTransientService>();
services1.AddScoped<ITestScopedService, TestScopedService>();

var provider1 = services1.BuildServiceProvider();

/* 
Singleton example:
- Both variables should have the SAME Id
- Because only ONE instance is created and reused
*/
var testSingletonService1 = provider1.GetRequiredService<ITestSingletonService>();
var testSingletonService2 = provider1.GetRequiredService<ITestSingletonService>();
Console.WriteLine("Singleton Example:");
Console.WriteLine($"Instance 1: {testSingletonService1.Id}");
Console.WriteLine($"Instance 1: {testSingletonService2.Id}");
Console.WriteLine();

/*
Transient example:
- Each request creates a NEW instance
- So Ids will be DIFFERENT
*/
var testTransientService1 = provider1.GetRequiredService<ITestTransientService>();
var testTransientService2 = provider1.GetRequiredService<ITestTransientService>();
Console.WriteLine("Transient Example:");
Console.WriteLine($"Instance 1: {testTransientService1.Id}");
Console.WriteLine($"Instance 1: {testTransientService2.Id}");
Console.WriteLine();

/* 
Scoped example
- Same instance WITHIN a scope
- Different instance ACROSS scopes
*/
Console.WriteLine("Scoped Example:");
using(var scope = provider1.CreateScope())
{
    var testScopedService1 = scope.ServiceProvider.GetRequiredService<ITestScopedService>(); 
    var testScopedService2 = scope.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService3 = scope.ServiceProvider.GetRequiredService<ITestScopedService>(); 
    var testScopedService4 = scope.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService5 = scope.ServiceProvider.GetRequiredService<ITestScopedService>();

    Console.WriteLine("Scoped 1: ");
    Console.WriteLine($"Instance 1 From Scope 2: {testScopedService1.Id}");
    Console.WriteLine($"Instance 2 From Scope 2: {testScopedService2.Id}");
    Console.WriteLine($"Instance 3 From Scope 2: {testScopedService3.Id}");
    Console.WriteLine($"Instance 4 From Scope 2: {testScopedService4.Id}");
    Console.WriteLine($"Instance 5 From Scope 2: {testScopedService5.Id}");
}

using(var scope1 = provider1.CreateScope())
{
    var testScopedService6 = scope1.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService7 = scope1.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService8 = scope1.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService9 = scope1.ServiceProvider.GetRequiredService<ITestScopedService>();
    var testScopedService10 = scope1.ServiceProvider.GetRequiredService<ITestScopedService>();

    Console.WriteLine("Scoped 2: ");
    Console.WriteLine($"Instance 1 From Scope 2: {testScopedService6.Id}");
    Console.WriteLine($"Instance 2 From Scope 2: {testScopedService7.Id}");
    Console.WriteLine($"Instance 3 From Scope 2: {testScopedService8.Id}");
    Console.WriteLine($"Instance 4 From Scope 2: {testScopedService9.Id}");
    Console.WriteLine($"Instance 5 From Scope 2: {testScopedService10.Id}");
}
Console.WriteLine();

// Real Life scenarios
Console.WriteLine("Real Life Scenarios:");
var services2 = new ServiceCollection();
services2.AddTransient<PaymentService>();
services2.AddTransient<INotificationService, EmaillService>();

var provider2 = services2.BuildServiceProvider();
var paymentService = provider2.GetRequiredService<PaymentService>();
paymentService.Process();
