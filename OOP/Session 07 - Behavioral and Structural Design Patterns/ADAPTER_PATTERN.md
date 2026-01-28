# Payment Gateway Adapter - Simple Bangladesh Example

## The Problem: bKash and Rocket have different interfaces

```csharp
using System;

// ============================================
// THIRD-PARTY PAYMENT GATEWAYS
// ============================================

// bKash API
public class BkashAPI
{
    public string SendMoney(string phoneNumber, double amount)
    {
        Console.WriteLine($"bKash: Sending {amount} BDT to {phoneNumber}");
        string trxId = $"BK{new Random().Next(10000, 99999)}";
        Console.WriteLine($"bKash: Transaction successful!");
        return trxId;
    }
}

// Rocket API
public class RocketAPI
{
    public bool Pay(string accountNumber, decimal taka)
    {
        Console.WriteLine($"Rocket: Processing {taka} BDT from {accountNumber}");
        Console.WriteLine($"Rocket: Payment completed!");
        return true;
    }
    
    public string GetTransactionRef()
    {
        return $"RKT-{new Random().Next(1000, 9999)}";
    }
}
```

**The Problem:**
- bKash: `SendMoney(string, double)` → returns transaction ID directly
- Rocket: `Pay(string, decimal)` → returns boolean, then call `GetTransactionRef()`
- **Different methods, different return types!** 😫

## Requirement

**We want to centralize all payment systems into one common gateway for our application.**

---

## The Solution: Common Interface + Adapters

```csharp
using System;

// ============================================
// YOUR COMMON INTERFACE
// ============================================

public interface IPaymentGateway
{
    string Pay(string account, decimal amount);
}

// ============================================
// ADAPTERS
// ============================================

// bKash Adapter
public class BkashAdapter : IPaymentGateway
{
    private BkashAPI bkash = new BkashAPI();
    
    public string Pay(string account, decimal amount)
    {
        // Convert decimal to double for bKash
        string transactionId = bkash.SendMoney(account, (double)amount);
        return transactionId;
    }
}

// Rocket Adapter
public class RocketAdapter : IPaymentGateway
{
    private RocketAPI rocket = new RocketAPI();
    
    public string Pay(string account, decimal amount)
    {
        // Call Rocket's Pay method
        bool success = rocket.Pay(account, amount);
        
        // Get transaction reference
        if (success)
        {
            return rocket.GetTransactionRef();
        }
        
        return null;
    }
}

// ============================================
// YOUR APPLICATION
// ============================================

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PAYMENT GATEWAY DEMO ===\n");
        
        // Customer 1: Pay with bKash
        Console.WriteLine("Customer 1 paying with bKash:");
        IPaymentGateway bkash = new BkashAdapter();
        string txn1 = bkash.Pay("01712345678", 1500);
        Console.WriteLine($"Transaction ID: {txn1}\n");
        
        Console.WriteLine(new string('-', 50) + "\n");
        
        // Customer 2: Pay with Rocket
        Console.WriteLine("Customer 2 paying with Rocket:");
        IPaymentGateway rocket = new RocketAdapter();
        string txn2 = rocket.Pay("01898765432", 2500);
        Console.WriteLine($"Transaction ID: {txn2}\n");
        
        Console.WriteLine(new string('-', 50) + "\n");
        
        // The beauty: Both use the same interface!
        Console.WriteLine("Processing multiple payments:");
        ProcessPayment(new BkashAdapter(), "01711111111", 500);
        ProcessPayment(new RocketAdapter(), "01822222222", 750);
        ProcessPayment(new BkashAdapter(), "01733333333", 1000);
    }
    
    static void ProcessPayment(IPaymentGateway gateway, string account, decimal amount)
    {
        Console.WriteLine($"\nProcessing {amount} BDT...");
        string txnId = gateway.Pay(account, amount);
        Console.WriteLine($"✓ Success! Transaction: {txnId}");
    }
}
```

---

## Why This Works

### Before (Without Adapter):
```csharp
// You have to remember different methods for each gateway
if (paymentMethod == "bkash")
{
    BkashAPI bkash = new BkashAPI();
    string txn = bkash.SendMoney(phone, amount);  // Different method!
}
else if (paymentMethod == "rocket")
{
    RocketAPI rocket = new RocketAPI();
    bool success = rocket.Pay(account, amount);   // Different method!
    string txn = rocket.GetTransactionRef();      // Extra step!
}
```

### After (With Adapter):
```csharp
// Same method for all gateways!
IPaymentGateway gateway = GetGateway(paymentMethod);
string txn = gateway.Pay(account, amount);  // Same for all! ✨
```

---

## Key Points

✅ **One Interface** - `IPaymentGateway` with `Pay()` method

✅ **Two Adapters** - `BkashAdapter` and `RocketAdapter` translate the different APIs

✅ **Your Code** - Only knows about `IPaymentGateway`, doesn't care about bKash or Rocket details

✅ **Easy to Add** - Want Nagad? Just create `NagadAdapter`!

The adapter pattern makes different payment gateways work the same way! 🎉

---

## 🎁 BONUS: Adding Factory Pattern for Even Cleaner Code

Now that you understand the Adapter pattern, let's make the code even better with the **Factory Pattern**!

### The Problem with Current Code:
```csharp
// You still have to manually create adapters
IPaymentGateway gateway;

if (method == "bkash")
{
    gateway = new BkashAdapter();
}
else if (method == "rocket")
{
    gateway = new RocketAdapter();
}
else
{
    throw new Exception("Unknown gateway");
}

// This gets messy when you have many gateways!
```

### Solution: Factory Pattern

```csharp
// ============================================
// FACTORY PATTERN
// ============================================

public class PaymentGatewayFactory
{
    public static IPaymentGateway GetGateway(string gatewayType)
    {
        switch (gatewayType.ToLower())
        {
            case "bkash":
                return new BkashAdapter();
            
            case "rocket":
                return new RocketAdapter();
            
            default:
                throw new Exception($"Unknown gateway: {gatewayType}");
        }
    }
}

// ============================================
// UPDATED APPLICATION WITH FACTORY
// ============================================

class ProgramWithFactory
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PAYMENT GATEWAY WITH FACTORY ===\n");
        
        // Interactive example
        Console.Write("Choose payment method (bkash/rocket): ");
        string userChoice = Console.ReadLine();
        
        try
        {
            IPaymentGateway userGateway = PaymentGatewayFactory.GetGateway(userChoice);
            ProcessPayment(userGateway, "01799999999", 999);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    static void ProcessPayment(IPaymentGateway gateway, string account, decimal amount)
    {
        Console.WriteLine($"Processing {amount} BDT...");
        string txnId = gateway.Pay(account, amount);
        Console.WriteLine($"✓ Success! Transaction: {txnId}");
    }
}
```

## Factory Pattern Benefits

### ✅ Centralized Object Creation
```csharp
// All creation logic in one place
public static IPaymentGateway GetGateway(string gatewayType)
{
    // Add new gateway here
    // No need to change code everywhere!
}
```

### ✅ Clean Client Code
```csharp
// Before Factory:
IPaymentGateway gateway;
if (method == "bkash") gateway = new BkashAdapter();
else if (method == "rocket") gateway = new RocketAdapter();
// ... messy if-else chains

// After Factory:
IPaymentGateway gateway = PaymentGatewayFactory.GetGateway(method);
// One line! ✨
```

### ✅ Easy to Extend
```csharp
// Want to add Nagad?

// Step 1: Create adapter
public class NagadAdapter : IPaymentGateway
{
    public string Pay(string account, decimal amount)
    {
        // Implementation
    }
}

// Step 2: Update factory (only one place!)
public static IPaymentGateway GetGateway(string gatewayType)
{
    switch (gatewayType.ToLower())
    {
        case "bkash":
            return new BkashAdapter();
        case "rocket":
            return new RocketAdapter();
        case "nagad":              // Add this
            return new NagadAdapter();  // And this!
        default:
            throw new Exception($"Unknown gateway: {gatewayType}");
    }
}

// Done! Works everywhere automatically
```

### ✅ Runtime Flexibility
```csharp
// Create gateways based on user input or configuration
string method = GetUserChoice(); // From UI
IPaymentGateway gateway = PaymentGatewayFactory.GetGateway(method);

// Or from database/config
string method = config.GetPaymentMethod();
IPaymentGateway gateway = PaymentGatewayFactory.GetGateway(method);
```

---

## Summary: Adapter + Factory = Perfect Combination!

**Adapter Pattern** solves: Different interfaces problem
**Factory Pattern** solves: Object creation complexity

Together they give you:
- ✅ Uniform interface (Adapter)
- ✅ Easy object creation (Factory)
- ✅ Flexible, maintainable code
- ✅ Simple to add new gateways

```csharp
// Two powerful patterns working together:
IPaymentGateway gateway = PaymentGatewayFactory.GetGateway("bkash");
string txn = gateway.Pay(account, amount);

// Clean, simple, powerful! 🚀
```
