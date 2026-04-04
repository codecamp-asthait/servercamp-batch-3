public interface ITestSingletonService
{
    public Guid Id { get; }
}

public class TestSingletonService : ITestSingletonService
{
    public Guid Id { get; } = Guid.NewGuid(); // Globally Unique Identifier
}

public interface ITestTransientService
{
    public Guid Id { get; }
}

public class TestTransientService : ITestTransientService
{
    public Guid Id { get; } = Guid.NewGuid(); // Globally Unique Identifier
}

public interface ITestScopedService
{
    public Guid Id { get; }
}

public class TestScopedService : ITestScopedService
{
    public Guid Id { get; } = Guid.NewGuid(); // Globally Unique Identifier
}

public class PaymentService
{
    private readonly INotificationService _notificationService;

    public PaymentService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void Process()
    {
        Console.WriteLine("Payment process completed");
        _notificationService.Send();
    }
}

public interface INotificationService
{
    public void Send();
}

public class EmaillService : INotificationService
{
    public void Send()
    {
        Console.WriteLine("Email sent.");
    }
}


public class Parent
{
    public Parent(
        Child1 child1,
        Child2 child2,
        Child3 child3,
        Child4 child4,
        Child5 child5
    )
    {

    }
}

public class Child1
{
    public Child1(
        ChildChild1 childChild1,
        ChildChild2 childChild2,
        ChildChild3 childChild3
    )
    {

    }
}

public class Child2 { }

public class Child3 { }

public class Child4 { }

public class Child5 { }

public class ChildChild1
{
    public ChildChild1(
        ChildChild2 childChild2,
        ChildChild3 childChild3
    )
    {

    }
}

public class ChildChild2 { }

public class ChildChild3 { }