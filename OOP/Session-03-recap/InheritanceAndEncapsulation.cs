using System;
using System.Collections.Generic;

namespace OOP.Session03
{
    // Different Types of Inheritance

    // 1. Single Inheritance
    public class Animal
    {
        public void Eat()
        {
            Console.WriteLine("This animal eats food.");
        }
    }

    public class Dog : Animal
    {
        public void Bark()
        {
            Console.WriteLine("The dog barks.");
        }
    }

    // 2. Multilevel Inheritance
    public class Puppy : Dog
    {
        public void Weep()
        {
            Console.WriteLine("The puppy weeps.");
        }
    }

    // 3. Hierarchical Inheritance
    public class Cat : Animal
    {
        public void Meow()
        {
            Console.WriteLine("The cat meows.");
        }
    }

    // 4. Multiple Inheritance (via Interfaces in C#)
    public interface ICanFly
    {
        void Fly();
    }

    public class Bird : Animal, ICanFly
    {
        public void Fly()
        {
            Console.WriteLine("The bird flies.");
        }
    }

    // 5. Hybrid Inheritance (Combination of Inheritance and Interfaces)
    public interface IEngine
    {
        void StartEngine();
    }

    public class Vehicle
    {
        public void Drive()
        {
            Console.WriteLine("The vehicle is driving.");
        }
    }

    public class Car : Vehicle, IEngine
    {
        public void StartEngine()
        {
            Console.WriteLine("The car engine starts.");
        }
    }

    public class ElectricCar : Car
    {
        public void ChargeBattery()
        {
            Console.WriteLine("The electric car is charging.");
        }
    }

    // Production-Level Example of Inheritance
    public abstract class PaymentProcessor
    {
        public string TransactionId { get; protected set; }
        public decimal Amount { get; protected set; }

        protected PaymentProcessor(decimal amount)
        {
            Amount = amount;
            TransactionId = Guid.NewGuid().ToString();
        }

        public abstract bool ProcessPayment();

        public virtual void LogTransaction(string details)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] Transaction {TransactionId}: {details}");
        }
    }

    public class CreditCardPaymentProcessor : PaymentProcessor
    {
        private readonly string _cardNumber;

        public CreditCardPaymentProcessor(decimal amount, string cardNumber) : base(amount)
        {
            _cardNumber = cardNumber;
        }

        public override bool ProcessPayment()
        {
            LogTransaction("Processing credit card payment.");
            return true; // Simulate success
        }
    }

    public class PayPalPaymentProcessor : PaymentProcessor
    {
        private readonly string _email;

        public PayPalPaymentProcessor(decimal amount, string email) : base(amount)
        {
            _email = email;
        }

        public override bool ProcessPayment()
        {
            LogTransaction("Processing PayPal payment.");
            return true; // Simulate success
        }
    }

    // Encapsulation Example
    public class BankAccount
    {
        private decimal _balance;

        public decimal Balance
        {
            get { return _balance; }
            private set { _balance = value; }
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");

            _balance += amount;
            Console.WriteLine($"Deposited {amount:C}. New balance: {_balance:C}");
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");

            if (amount > _balance)
                throw new InvalidOperationException("Insufficient funds.");

            _balance -= amount;
            Console.WriteLine($"Withdrew {amount:C}. Remaining balance: {_balance:C}");
        }
    }

    // Test Program
    public class Program
    {
        public static void Main(string[] args)
        {
            // Inheritance Examples
            Dog dog = new Dog();
            dog.Eat();
            dog.Bark();

            Puppy puppy = new Puppy();
            puppy.Eat();
            puppy.Bark();
            puppy.Weep();

            Cat cat = new Cat();
            cat.Eat();
            cat.Meow();

            Bird bird = new Bird();
            bird.Eat();
            bird.Fly();

            // Production-Level Inheritance Example
            PaymentProcessor creditCardPayment = new CreditCardPaymentProcessor(100.50m, "4111111111111111");
            creditCardPayment.ProcessPayment();

            PaymentProcessor paypalPayment = new PayPalPaymentProcessor(200.75m, "user@example.com");
            paypalPayment.ProcessPayment();

            // Encapsulation Example
            BankAccount account = new BankAccount();
            account.Deposit(500);
            account.Withdraw(200);

            // Hybrid Inheritance Example
            ElectricCar tesla = new ElectricCar();
            tesla.StartEngine();
            tesla.Drive();
            tesla.ChargeBattery();
        }
    }
}