using System;

namespace LSPExample;
// ===============================
// ✅ WITH LSP
// ===============================

// Only ONLINE payments
public abstract class OnlinePayment
{
    public abstract void PayOnline(decimal amount);
}

public class CardPayment : OnlinePayment
{
    public override void PayOnline(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using Card");
    }
}

public class BkashPayment : OnlinePayment
{
    public override void PayOnline(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using bKash");
    }
}

// Separate class for offline payment
public class CashOnDelivery
{
    public void PayOnDelivery(decimal amount)
    {
        Console.WriteLine($"Cash {amount} will be collected on delivery");
    }
}

public class OnlinePaymentProcessor
{
    public void Process(OnlinePayment payment, decimal amount)
    {
        payment.PayOnline(amount); // ✅ Always safe
    }
}

class Program
{
    static void Main()
    {
        OnlinePaymentProcessor processor = new OnlinePaymentProcessor();

        OnlinePayment card = new CardPayment();
        processor.Process(card, 1000); // ✅

        OnlinePayment bkash = new BkashPayment();
        processor.Process(bkash, 500); // ✅


        // Cash on Delivery handled separately
        CashOnDelivery cod = new CashOnDelivery();
        cod.PayOnDelivery(800); // ✅ handled separately
    }
}

