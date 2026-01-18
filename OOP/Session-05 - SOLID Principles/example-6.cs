using System;

namespace ISP_SingleFile;
// ===============================
// ✅ SMALL, FOCUSED INTERFACES
// ===============================
public interface IEmailNotifier
{
    void SendEmail(string message);
}

public interface ISmsNotifier
{
    void SendSms(string message);
}

public interface IPushNotifier
{
    void SendPush(string message);
}

// ===============================
// ✅ IMPLEMENTATIONS
// ===============================
public class EmailNotificationService : IEmailNotifier
{
    public void SendEmail(string message)
    {
        Console.WriteLine($"Email sent: {message}");
    }
}

public class SmsNotificationService : ISmsNotifier
{
    public void SendSms(string message)
    {
        Console.WriteLine($"SMS sent: {message}");
    }
}

public class PushNotificationService : IPushNotifier
{
    public void SendPush(string message)
    {
        Console.WriteLine($"Push notification sent: {message}");
    }
}

// ===============================
// ✅ HIGH-LEVEL SERVICE
// Depends ONLY on what it needs
// ===============================
public class OrderService
{
    private readonly IEmailNotifier _emailNotifier;

    public OrderService(IEmailNotifier emailNotifier)
    {
        _emailNotifier = emailNotifier;
    }

    public void PlaceOrder()
    {
        Console.WriteLine("Order placed successfully");
        _emailNotifier.SendEmail("Your order has been confirmed");
    }
}

// ===============================
// Program Entry
// ===============================
class Program
{
    static void Main()
    {
        // Email use case
        IEmailNotifier emailService = new EmailNotificationService();
        OrderService orderService = new OrderService(emailService);
        orderService.PlaceOrder();

        // SMS use case
        ISmsNotifier smsService = new SmsNotificationService();
        smsService.SendSms("Your OTP is 123456");

        // Push use case
        IPushNotifier pushService = new PushNotificationService();
        pushService.SendPush("New update available!");
    }
}

