/*
This class demonstrates how dependencies are manually resolved 
without using Dependency Injection (DI).

Method: DependencyResolveWithoutDI()
Step 1: Create nested child objects
- ChildChild2 and ChildChild3 are instantiated first.
- These are passed into ChildChild1 constructor.
  ChildChild1 depends on:
    - ChildChild2
    - ChildChild3

Step 2: Create Child1
- Child1 depends on:
    - ChildChild1 (created in step 1)
    - ChildChild2 (new instance)
    - ChildChild3 (new instance)
- Notice how ChildChild2 and ChildChild3 are created multiple times manually.

Step 3: Create Parent
- Parent depends on:
    - Child1 (created in step 2)
    - Child2 (new instance)
    - Child3 (new instance)
    - Child4 (new instance)
    - Child5 (new instance)

Step 4: Result
- A single Parent instance is created with all nested dependencies.
- Every dependency must be manually instantiated and passed into constructors.
- This demonstrates "constructor explosion" and tight coupling.

Step 5: Problems Highlighted
- Hard to read: nested new statements create deeply indented code.
- Hard to maintain: adding or changing dependencies requires changes everywhere.
- Hard to test: mocking or substituting dependencies is difficult.
- Repetition: ChildChild2 and ChildChild3 are created multiple times unnecessarily.

Step 6: Takeaway
- This is exactly the kind of scenario where Dependency Injection (DI)
  and Inversion of Control (IoC) shine.
- Using DI, an IoC container would automatically create and inject all dependencies,
  reducing code complexity and making testing easier.
*/

using Microsoft.Extensions.DependencyInjection;

public class DIComplexityDemo
{
    public void DependencyResolveWithoutDI()
    {
        var parent = new Parent(
            new Child1(
                new ChildChild1(
                    new ChildChild2(),
                    new ChildChild3()
                ),
                new ChildChild2(),
                new ChildChild3()
            ),
            new Child2(),
            new Child3(),
            new Child4(),
            new Child5()
        );
    }

    public void DependencyResolveWithDI()
    {
        // Create a ServiceCollection (DI container)
        var services = new ServiceCollection();

        // Register all dependencies
        // Register classes as transient (default, new instance every time)
        services.AddTransient<Parent>();
        services.AddTransient<Child1>();
        services.AddTransient<Child2>();
        services.AddTransient<Child3>();
        services.AddTransient<Child4>();
        services.AddTransient<Child5>();
        services.AddTransient<ChildChild1>();
        services.AddTransient<ChildChild2>();
        services.AddTransient<ChildChild3>();

        // Build the service provider
        var provider = services.BuildServiceProvider();

        // Resolve the top-level dependency (Parent)
        // All dependencies are automatically created and injected by the container
        var parent = provider.GetRequiredService<Parent>();
    }
}