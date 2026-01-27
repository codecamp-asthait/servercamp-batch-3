Console.WriteLine("Choose: 1. Bkash  2. Rocket  3. Card 4. CoD");
string choice = Console.ReadLine();
MakePayment(choice, 100); // Bkash, 100

static void MakePayment(string method, decimal amount)
{
    // With Factory Pattern, we simplify object creation following Open/Closed Principle
    var paymentMethod = PaymentFactory.CreatePaymentMethod(method);
    paymentMethod.Pay(amount);
}

class PaymentFactory
{
    public static IPaymentMethod CreatePaymentMethod(string method)
    {
        if (method == "Bkash")
        {
            return new BkashPayment();
        }
        else if (method == "Rocket")
        {
            return new RocketPayment();
        }
        else if (method == "Card")
        {
            return new CardPayment();
        }
        else if (method == "CoD")
        {
            return new CashOnDelivery();
        }
        else
        {
            throw new ArgumentException("Invalid payment method");
        }
    }
}

interface IPaymentMethod
{
    void Pay(decimal amount);
}

class CashOnDelivery : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Cash on Delivery");
    }
}

class BkashPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Bkash");
    }
}

class RocketPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Rocket");
    }
}

class CardPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Card");
    }
}
