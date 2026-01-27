using System;

public class OrderService
{
    // ❌ One method handling too many responsibilities:
    // - Order processing
    // - Payment handling
    // - Notifications
    // - Database & invoice logic
    public void ProcessOrder(
        int orderId,
        decimal amount,
        string paymentMethod,
        string email,
        string phone)
    {
        Console.WriteLine($"Processing order {orderId}");

        // ❌ OCP violation:
        // Every new payment method requires modifying this method
        if (paymentMethod == "Card")
        {
            Console.WriteLine($"Charging card with amount {amount}");
            Console.WriteLine("Connecting to card gateway...");
            Console.WriteLine("Card payment successful");
        }
        else if (paymentMethod == "Cash")
        {
            Console.WriteLine("Cash will be collected on delivery");
        }
        else if (paymentMethod == "Crypto") // ❌ YAGNI (not actually required)
        {
            Console.WriteLine("Processing crypto payment...");
        }

        // ❌ DRY violation:
        // Notification logic duplicated
        Console.WriteLine($"Sending email to {email}");
        Console.WriteLine("Email sent successfully");

        Console.WriteLine($"Sending SMS to {phone}");
        Console.WriteLine("SMS sent successfully");

        // ❌ SRP violation:
        // OrderService should not know database details
        SaveOrderToDatabase(orderId, amount);

        // ❌ Unnecessary responsibility
        GenerateInvoice(orderId);
    }

    private void SaveOrderToDatabase(int orderId, decimal amount)
    {
        // ❌ Database logic mixed with business logic
        Console.WriteLine($"Saving order {orderId} with amount {amount}");
    }

    private void GenerateInvoice(int orderId)
    {
        // ❌ Invoice generation does not belong here
        Console.WriteLine($"Generating invoice for order {orderId}");
    }
}

// ✅ Main program to run the OrderService
class Program
{
    static void Main(string[] args)
    {
        // Create instance of OrderService
        OrderService orderService = new OrderService();

        // Process a sample order
        int orderId = 101;
        decimal amount = 500.75m;
        string paymentMethod = "Card";  // Try "Cash" or "Crypto"
        string email = "customer@example.com";
        string phone = "+880123456789";

        orderService.ProcessOrder(orderId, amount, paymentMethod, email, phone);

        // Wait for user input to see output (optional)
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
