using System;

// ✅ Abstraction for payment logic
// OrderService depends on abstraction, not concrete class (DIP)
public interface IPaymentService
{
    void Pay(decimal amount);
}

// ✅ Abstraction for notification logic
public interface INotificationService
{
    void Notify(string message);
}

// ================= Payment Implementations =================

// ✅ Single Responsibility: Handles ONLY card payment
public class CardPaymentService : IPaymentService
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using card");
    }
}


// cash on delivery
public class CashOnDelivery : Payment
{
    public override void Pay(decimal amount)
    {
        // ❌ Breaking expectation
        throw new NotSupportedException("Cash cannot be paid online");
    }
}

// ================= Notification Implementations =================

// ✅ Handles ONLY email notifications
public class EmailNotificationService : INotificationService
{
    public void Notify(string message)
    {
        Console.WriteLine($"Email notification: {message}");
    }
}

// ✅ Handles ONLY SMS notifications
public class SmsNotificationService : INotificationService
{
    public void Notify(string message)
    {
        Console.WriteLine($"SMS notification: {message}");
    }
}

// ================= Order Service =================

public class OrderService
{
    private readonly IPaymentService _paymentService;
    private readonly INotificationService _notificationService;

    // ✅ Dependency Injection (DIP + IoC friendly)
    public OrderService(
        IPaymentService paymentService,
        INotificationService notificationService)
    {
        _paymentService = paymentService;
        _notificationService = notificationService;
    }

    // ✅ KISS: Simple, readable, focused method
    public void ProcessOrder(int orderId, decimal amount)
    {
        Console.WriteLine($"Processing order {orderId}");

        // ✅ Delegating responsibilities
        _paymentService.Pay(amount);
        _notificationService.Notify($"Order {orderId} processed successfully");
    }
}

// ================= Main Program =================

class Program
{
    static void Main(string[] args)
    {
        // Using card payment and email notification
        IPaymentService payment = new CardPaymentService();
        INotificationService notification = new EmailNotificationService();

        OrderService orderService = new OrderService(payment, notification);

        // Process order
        int orderId = 101;
        decimal amount = 500.75m;

        orderService.ProcessOrder(orderId, amount);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
