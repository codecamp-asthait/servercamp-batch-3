using System;
using System.Collections.Generic;

namespace OCPExample;
// ===============================
// ❌ WITHOUT OCP
// ===============================
public class OrderWithoutOCP
{
    public string Product { get; set; }
    public decimal Weight { get; set; } // in kg
}

public class ShippingCalculatorWithoutOCP
{
    public decimal CalculateShipping(OrderWithoutOCP order, string shippingType)
    {
        decimal cost = 0;

        if (shippingType == "Standard")
        {
            cost = order.Weight * 5; // $5 per kg
        }
        else if (shippingType == "Express")
        {
            cost = order.Weight * 10; // $10 per kg
        }

        return cost;
    }
}

// ===============================
// ✅ WITH OCP
// ===============================
public class Order
{
    public string Product { get; set; }
    public decimal Weight { get; set; } // in kg
}

// Abstraction
public abstract class ShippingMethod
{
    public abstract decimal Calculate(Order order);
}

// Concrete shipping methods
public class StandardShipping : ShippingMethod
{
    public override decimal Calculate(Order order)
    {
        return order.Weight * 5;
    }
}

public class ExpressShipping : ShippingMethod
{
    public override decimal Calculate(Order order)
    {
        return order.Weight * 10;
    }
}

public class ShippingProcessor
{
    public decimal GetShippingCost(Order order, ShippingMethod method)
    {
        return method.Calculate(order);
    }
}

// ===============================
// Program Entry
// ===============================
class Program
{
    static void Main()
    {
        Console.WriteLine("=== WITHOUT OCP ===");
        OrderWithoutOCP oldOrder = new OrderWithoutOCP { Product = "Laptop", Weight = 2 };
        ShippingCalculatorWithoutOCP oldCalculator = new ShippingCalculatorWithoutOCP();
        decimal oldCost = oldCalculator.CalculateShipping(oldOrder, "Express");
        Console.WriteLine($"Shipping cost: {oldCost}");

        Console.WriteLine("\n=== WITH OCP ===");
        Order order = new Order { Product = "Laptop", Weight = 2 };
        ShippingProcessor processor = new ShippingProcessor();

        ShippingMethod standard = new StandardShipping();
        Console.WriteLine($"Standard Shipping: {processor.GetShippingCost(order, standard)}");

        ShippingMethod express = new ExpressShipping();
        Console.WriteLine($"Express Shipping: {processor.GetShippingCost(order, express)}");

        // ✅ Add new method without modifying old code
        ShippingMethod drone = new DroneShipping();
        Console.WriteLine($"Drone Shipping: {processor.GetShippingCost(order, drone)}");
    }
}

// New shipping method added easily
public class DroneShipping : ShippingMethod
{
    public override decimal Calculate(Order order)
    {
        return order.Weight * 20; // $20 per kg
    }
}
