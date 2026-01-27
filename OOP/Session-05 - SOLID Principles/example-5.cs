using System;

namespace ISP_SingleFile;

// ❌ Fat interface
public interface INotificationService
{
    void SendEmail(string message);
    void SendSms(string message);
    void SendPush(string message);
}

public class EmailNotificationService : INotificationService
{
    public void SendEmail(string message)
    {
        Console.WriteLine($"Email sent: {message}");
    }

    public void SendSms(string message)
    {
        throw new NotSupportedException("SMS not supported");
    }

    public void SendPush(string message)
    {
        throw new NotSupportedException("Push not supported");
    }
}

// ✅ ISP-compliant interfaces
public interface IEmailNotificationService
{
    void SendEmail(string message);
}

public interface ISmsNotificationService
{
    void SendSms(string message);
}

public interface IPushNotificationService
{
    void SendPush(string message);
}

public class BetterEmailNotificationService : IEmailNotificationService
{
    public void SendEmail(string message)
    {
        Console.WriteLine($"[ISP] Email sent: {message}");
    }
}

public class SmsNotificationService : ISmsNotificationService
{
    public void SendSms(string message)
    {
        Console.WriteLine($"[ISP] SMS sent: {message}");
    }
}

public class PushNotificationService : IPushNotificationService
{
    public void SendPush(string message)
    {
        Console.WriteLine($"[ISP] Push sent: {message}");
    }
}

// Demo entry point
class Program
{
    static void Main(string[] args)
    {
        IEmailNotificationService emailService = new BetterEmailNotificationService();
        ISmsNotificationService smsService = new SmsNotificationService();
        IPushNotificationService pushService = new PushNotificationService();

        emailService.SendEmail("Hello via Email!");
        smsService.SendSms("Hello via SMS!");
        pushService.SendPush("Hello via Push!");
    }
}

