Console.WriteLine("Choose: 1. Bkash  2. Rocket  3. Card 4. CoD");
string choice = Console.ReadLine();
MakePayment(choice, 100); // Bkash, 100

static void MakePayment(string method, decimal amount)
{
    // Without Factory Pattern, we break Open/Closed Principle

    if (method == "Bkash")
    {
        BkashPayment bkash = new BkashPayment();
        bkash.Pay(amount);
    }
    else if (method == "Rocket")
    {
        RocketPayment rocket = new RocketPayment();
        rocket.Pay(amount);
    }
    else if (method == "Card")
    {
        CardPayment card = new CardPayment();
        card.Pay(amount);
    }
    else if (method == "CoD")
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Cash on Delivery");
    }
    else
    {
        Console.WriteLine("Invalid payment method");
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
