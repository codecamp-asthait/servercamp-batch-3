using System;

namespace LSPExample;
// ===============================
// ❌ WITHOUT LSP
// ===============================
public abstract class Payment
{
    public abstract void Pay(decimal amount);
}

public class CardPayment : Payment
{
    public override void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using Card");
    }
}

public class CashOnDelivery : Payment
{
    public override void Pay(decimal amount)
    {
        // ❌ Breaking expectation
        throw new NotSupportedException("Cash cannot be paid online");
    }
}

public class PaymentProcessor
{
    public void Process(Payment payment, decimal amount)
    {
        payment.Pay(amount); // 💥 may crash
    }
}

class Program
{
    static void Main()
    {
        PaymentProcessor processor = new PaymentProcessor();

        Payment card = new CardPayment();
        processor.Process(card, 1000); // ✅ Works

        Payment cod = new CashOnDelivery();
        processor.Process(cod, 1000); // ❌ Runtime error
    }
}

