using System;

// Abstract base class
abstract class Payment
{
    // Abstract method: must be implemented by derived class
    public abstract void Pay(decimal amount);

    // Concrete method: optional, inherited as-is
    public void PrintReceipt()
    {
        Console.WriteLine("Receipt printed");
    }

    // Virtual method: optional override
    public virtual void ApplyDiscount()
    {
        Console.WriteLine("Default discount applied");
    }
}

// Derived class: CardPayment
class CardPayment : Payment
{
    // Must override abstract method
    public override void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using card");
    }

    // Optional: override virtual method
    public override void ApplyDiscount()
    {
        Console.WriteLine("10% Card discount applied");
    }
}

// Derived class: PayPalPayment
class PayPalPayment : Payment
{
    public override void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} via PayPal");
    }

    // Does NOT override ApplyDiscount → uses default from base
}

// class Program
// {
//     static void Main()
//     {
//         Payment card = new CardPayment();
//         card.Pay(500);           // Uses CardPayment implementation
//         card.PrintReceipt();     // Inherited concrete method
//         card.ApplyDiscount();    // Overridden virtual method

//         Console.WriteLine();

//         Payment paypal = new PayPalPayment();
//         paypal.Pay(300);         // Uses PayPalPayment implementation
//         paypal.PrintReceipt();   // Inherited concrete method
//         paypal.ApplyDiscount();  // Uses default from base class
//     }
// }


// Abstract method Pay() → forces derived classes to implement it
// Concrete method PrintReceipt() → optional, inherited as-is
// Virtual method ApplyDiscount() → optional, can override or use default