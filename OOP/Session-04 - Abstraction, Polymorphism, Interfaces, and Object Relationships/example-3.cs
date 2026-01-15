using System;
using System.Collections.Generic;

namespace Controller;
// Controller
class PaymentController
{
    private readonly PaymentService _paymentService;

    public PaymentController()
    {
        _paymentService = new PaymentService();
    }

    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine("Controller: Initiating payment process...");
        _paymentService.HandlePayment(amount);
    }
}

// Service
class PaymentService
{
    private readonly PaymentRepository _paymentRepository;

    public PaymentService()
    {
        _paymentRepository = new PaymentRepository();
    }

    public void HandlePayment(decimal amount)
    {
        Console.WriteLine("Service: Handling payment...");
        _paymentRepository.SavePayment(amount);
    }
}

// Repository
class PaymentRepository
{
    private readonly List<decimal> _paymentDatabase;

    public PaymentRepository()
    {
        _paymentDatabase = new List<decimal>();
    }

    public void SavePayment(decimal amount)
    {
        Console.WriteLine("Repository: Saving payment to database...");
        _paymentDatabase.Add(amount);
        Console.WriteLine($"Database: Payment of {amount} saved successfully.");
    }
}

// Main Program
/*
class Program
{
    static void Main()
    {
        PaymentController controller = new PaymentController();
        controller.ProcessPayment(1000);
    }
}
*/