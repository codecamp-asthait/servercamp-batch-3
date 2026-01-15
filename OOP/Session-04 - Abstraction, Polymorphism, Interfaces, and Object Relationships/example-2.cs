using System;

namespace InterfaceExample;

// ---------------------------
// When to Use Interfaces
// ---------------------------
// 1. Multiple inheritance: Classes can implement multiple interfaces.
// 2. Define a contract without behavior: Interfaces only define method signatures.
// 3. Unrelated classes: Any class can implement an interface, even if unrelated.

// ---------------------------
// Example Interface for Payment
// ---------------------------
// This interface defines what any payment class should do.
interface IPayment
{
    void Pay(decimal amount);       // Must implement
    void PrintReceipt();            // Must implement
}

// ---------------------------
// CardPayment Class
// ---------------------------
// Implements IPayment interface.
// Provides concrete implementation for Pay and PrintReceipt methods.
class CardPayment : IPayment
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using card");
    }

    public void PrintReceipt()
    {
        Console.WriteLine("Receipt printed");
    }
}

// ---------------------------
// MobilePayment Class
// ---------------------------
// Another class implementing the same interface.
// Shows flexibility: different behavior for Pay and PrintReceipt.
class MobilePayment : IPayment
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using mobile payment");
    }

    public void PrintReceipt()
    {
        Console.WriteLine("Mobile payment receipt printed");
    }
}

// ---------------------------
// Example of Multiple Inheritance using Interfaces
// ---------------------------
// Interface for sending notifications
interface INotification
{
    void SendNotification(string message); // Must implement
}

// AdvancedPayment implements both IPayment and INotification
// Demonstrates multiple interface inheritance
class AdvancedPayment : IPayment, INotification
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Paid {amount} using advanced payment method");
    }

    public void PrintReceipt()
    {
        Console.WriteLine("Advanced payment receipt printed");
    }

    public void SendNotification(string message)
    {
        Console.WriteLine($"Notification sent: {message}");
    }
}

// ---------------------------
// Main Program
// ---------------------------
// Demonstrates polymorphism and multiple inheritance

// class Program
// {
//     static void Main()
//     {
//         // Using interface reference for CardPayment
//         IPayment payment = new CardPayment();
//         payment.Pay(300);        // Calls CardPayment.Pay
//         payment.PrintReceipt();  // Calls CardPayment.PrintReceipt

//         Console.WriteLine();

//         // Using interface reference for MobilePayment
//         payment = new MobilePayment();
//         payment.Pay(150);        // Calls MobilePayment.Pay
//         payment.PrintReceipt();  // Calls MobilePayment.PrintReceipt

//         Console.WriteLine();

//         // Using AdvancedPayment which implements two interfaces
//         AdvancedPayment advancedPayment = new AdvancedPayment();
//         advancedPayment.Pay(500);                      // IPayment method
//         advancedPayment.PrintReceipt();                // IPayment method
//         advancedPayment.SendNotification("Payment successful"); // INotification method
//     }
// }

