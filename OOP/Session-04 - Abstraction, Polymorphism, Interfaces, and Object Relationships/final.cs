using System;
using System.Collections.Generic;

namespace PaymentGatewayExample
{
    // ------------------------
    // Models
    // ------------------------

    /// <summary>
    /// Represents a payment transaction.
    /// </summary>
    public class Transaction
    {
        public string TransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Status { get; private set; }

        public Transaction(decimal amount)
        {
            TransactionId = Guid.NewGuid().ToString();
            Amount = amount;
            Timestamp = DateTime.Now;
            Status = "Pending";
        }

        public void UpdateStatus(string status)
        {
            Status = status;
        }
    }

    // ------------------------
    // Services
    // ------------------------

    /// <summary>
    /// Interface for payment processors.
    /// </summary>
    public interface IPaymentProcessor
    {
        bool ProcessPayment(Transaction transaction);
    }

    /// <summary>
    /// A concrete implementation of a payment processor.
    /// </summary>
    public class StripePaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(Transaction transaction)
        {
            Console.WriteLine($"Processing payment of {transaction.Amount:C} through Stripe...");
            transaction.UpdateStatus("Success");
            return true;
        }
    }

    public class BkashPaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(Transaction transaction)
        {
            Console.WriteLine($"Processing payment of {transaction.Amount:C} through Bkash...");
            transaction.UpdateStatus("Success");
            return true;
        }
    }

    /// <summary>
    /// A concrete implementation of a payment processor.
    /// </summary>
    public class PayPalPaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(Transaction transaction)
        {
            Console.WriteLine($"Processing payment of {transaction.Amount:C} through PayPal...");
            transaction.UpdateStatus("Success");
            return true;
        }
    }

    // ------------------------
    // Core Payment Gateway
    // ------------------------

    /// <summary>
    /// The payment gateway that handles transactions.
    /// </summary>
    public class PaymentGateway
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentGateway(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public void MakePayment(decimal amount)
        {
            var transaction = new Transaction(amount);
            Console.WriteLine($"Initiating transaction {transaction.TransactionId}...");

            if (_paymentProcessor.ProcessPayment(transaction))
            {
                Console.WriteLine($"Transaction {transaction.TransactionId} completed successfully.");
            }
            else
            {
                Console.WriteLine($"Transaction {transaction.TransactionId} failed.");
            }
        }
    }

    // ------------------------
    // Program Entry
    // ------------------------

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Payment Gateway!");

            // Choose a payment processor (Stripe or PayPal)
            // IPaymentProcessor paymentProcessor = new StripePaymentProcessor();
            IPaymentProcessor paymentProcessor = new BkashPaymentProcessor();
            // IPaymentProcessor paymentProcessor = new PayPalPaymentProcessor();

            // Create the payment gateway
            var paymentGateway = new PaymentGateway(paymentProcessor);

            // Make a payment
            paymentGateway.MakePayment(100.50m);
        }
    }
}