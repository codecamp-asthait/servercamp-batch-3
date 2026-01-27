ShoppingCart cart = new ShoppingCart();

while (true)
{
    Console.WriteLine("\n=== Choose Payment ===");
    Console.WriteLine("1. Bkash");
    Console.WriteLine("2. Nagad");
    Console.WriteLine("3. Card");
    Console.WriteLine("4. Checkout");
    Console.WriteLine("0. Exit");

    string choice = Console.ReadLine();

    IPaymentMethod? paymentMethod = null;

    // Factory Pattern + Strategy Pattern
    switch (choice)
    {
        case "1":
            // Factory Pattern
            paymentMethod = PaymentFactory.CreatePaymentMethod("Bkash");
            // Strategy Pattern: sets payment method dynamically
            cart.SetPaymentMethod(paymentMethod);
            break;
        case "2":
            paymentMethod = PaymentFactory.CreatePaymentMethod("Rocket");
            cart.SetPaymentMethod(paymentMethod);
            break;
        case "3":
            paymentMethod = PaymentFactory.CreatePaymentMethod("Card"); // Factory Pattern
            cart.SetPaymentMethod(paymentMethod); // Strategy Pattern
            break;
        case "4":
            cart.Checkout(500);
            break;
        case "0":
            return;
    }
}
class ShoppingCart
{
    private IPaymentMethod? _paymentMethod = null; //bkash, card

    // Strategy Pattern
    public void SetPaymentMethod(IPaymentMethod paymentMethod) // BkashPayment
    {
        Console.WriteLine($"✅ Payment method set to {paymentMethod.GetType().Name}");
        _paymentMethod = paymentMethod;
    }

    public void Checkout(decimal amount)
    {
        if (_paymentMethod == null)
        {
            Console.WriteLine("❌ Payment method not set!");
            return;
        }
        _paymentMethod.Pay(amount); //card
    }
}

// == Factory Pattern Implementation ==

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
