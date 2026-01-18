using System;
namespace SRPExample;
// ===============================
// ❌ WITHOUT SRP
// ===============================
public class OrderManager
{
    public string CustomerEmail { get; set; }

    public void CreateOrder(string product, int quantity, decimal price)
    {
        // Calculate total
        decimal total = quantity * price;
        Console.WriteLine($"[Without SRP] Order created: {product} x {quantity} = {total}");

        // Save to database
        Console.WriteLine("[Without SRP] Saving order to database...");
        // Imagine actual database code here

        // Send email
        Console.WriteLine($"[Without SRP] Sending email to {CustomerEmail}...");
        // Imagine actual email code here
    }    
}

// ===============================
// ✅ WITH SRP
// ===============================
public class Order
{
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string CustomerEmail { get; set; }
}

public class OrderProcessor
{
    public decimal CalculateTotal(Order order)
    {
        return order.Quantity * order.Price;
    }
    // Calculate Taxes, Discounts, etc. can be added here
}

public class OrderRepository
{
    public void Save(Order order)
    {
        Console.WriteLine("[With SRP] Saving order to database...");
    }
    // Database interaction logic can be expanded here
}

public class EmailService
{
    public void SendConfirmation(string email, decimal total)
    {
        Console.WriteLine($"[With SRP] Sending email to {email} with total {total}");
    }
}

// ===============================
// Program Entry
// ===============================
class Program
{
    static void Main()
    {
        Console.WriteLine("=== WITHOUT SRP ===");
        OrderManager oldOrder = new OrderManager();
        oldOrder.CustomerEmail = "customer@example.com";
        oldOrder.CreateOrder("Laptop", 2, 500m);

        Console.WriteLine("\n=== WITH SRP ===");
        Order order = new Order
        {
            Product = "Laptop",
            Quantity = 2,
            Price = 500m,
            CustomerEmail = "customer@example.com"
        };

        OrderProcessor processor = new OrderProcessor();
        decimal total = processor.CalculateTotal(order);
        Console.WriteLine($"[With SRP] Order created: {order.Product} x {order.Quantity} = {total}");

        OrderRepository repository = new OrderRepository();
        repository.Save(order);

        EmailService emailService = new EmailService();
        emailService.SendConfirmation(order.CustomerEmail, total);
    }
}

