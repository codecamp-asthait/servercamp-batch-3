# 🚗 Ride Sharing System - C# Assignment

## 📱 What Are You Building?

You're going to build a **Ride Sharing App** like Uber or Pathao! You'll implement this step-by-step using design patterns and OOP principles.

---

## 🎯 Phase 1: Factory Pattern - Creating Vehicles

### 🤔 The Problem:
Creating different vehicle types with messy if-else statements.

### 📝 Your Tasks:

**Task 1.1:** Create vehicle interface and classes
```csharp
public interface IVehicle
{
    string GetVehicleType();
    double GetBaseFare();
}
```

Create these classes implementing `IVehicle`:
- **Bike** - Base fare: $2
- **CNG** - Base fare: $3
- **Car** - Base fare: $5

**Task 1.2:** Create `VehicleFactory` class
- Method: `IVehicle CreateVehicle(string vehicleType)`
- Should return appropriate vehicle based on type

**Task 1.3:** Create `Driver` class
```csharp
public class Driver
{
    public string Name { get; set; }
    public IVehicle Vehicle { get; set; }
    public bool IsAvailable { get; set; }
    
    // Add constructor and methods
}
```

### ✅ Test:
```csharp
VehicleFactory factory = new VehicleFactory();
Driver driver1 = new Driver("Ahmed", factory.CreateVehicle("Bike"));
Driver driver2 = new Driver("Karim", factory.CreateVehicle("CNG"));
```

**Expected:** Both drivers created with correct vehicles.

---

## 🎯 Phase 2: Inheritance - User Types

### 🤔 The Problem:
System has different types of users (Rider, Driver) with common properties but different behaviors.

### 📝 Your Tasks:

**Task 2.1:** Create base `User` class
```csharp
public abstract class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    
    // Add constructor
    
    public abstract void DisplayInfo();
    public abstract string GetRole();
}
```

**Task 2.2:** Create `Rider` class inheriting from `User`
- Additional property: `double WalletBalance`
- Override `DisplayInfo()` to show rider details
- Override `GetRole()` to return "Rider"
- Add method: `void RequestRide(string vehicleType)`

**Task 2.3:** Update existing `Driver` class to inherit from `User`
- Keep existing properties: `IVehicle Vehicle`, `bool IsAvailable`
- Override `DisplayInfo()` to show driver details
- Override `GetRole()` to return "Driver"
- Add method: `void AcceptRide(string rideId)`

### ✅ Test:
```csharp
User rider = new Rider("R001", "Fatima", "01722222222", 500.0);
User driver = new Driver("D001", "Ahmed", "01733333333", factory.CreateVehicle("CNG"));

rider.DisplayInfo();
driver.DisplayInfo();

Console.WriteLine($"Role: {rider.GetRole()}");
Console.WriteLine($"Role: {driver.GetRole()}");
```

**Expected:** All display correct info with proper roles.

---

## 🎯 Phase 3: Singleton Pattern - One Central Manager

### 🤔 The Problem:
Need ONE central system that all drivers and riders use.

### 📝 Your Tasks:

**Task 3.1:** Create `RideManager` as a Singleton
Requirements:
- Private constructor (prevent `new RideManager()`)
- Private static instance variable
- Public static `GetInstance()` method
- Store list of drivers
- Methods:
  - `void RegisterDriver(Driver driver)`
  - `List<Driver> GetAllDrivers()`
  - `List<Driver> GetAvailableDrivers(string vehicleType)`

**Hint:** Only ONE instance should ever exist!

### ✅ Test:
```csharp
RideManager manager1 = RideManager.GetInstance();
RideManager manager2 = RideManager.GetInstance();

manager1.RegisterDriver(driver1);
Console.WriteLine($"manager1 drivers: {manager1.GetAllDrivers().Count}");
Console.WriteLine($"manager2 drivers: {manager2.GetAllDrivers().Count}");
Console.WriteLine($"Same instance? {manager1 == manager2}");
```

**Expected:** 
- Both show 1 driver
- `Same instance? True`

---

## 🎯 Phase 4: Strategy Pattern - Different Pricing

### 🤔 The Problem:
Prices change based on time (rush hour, midnight, weekend).

### 📝 Your Tasks:

**Task 4.1:** Create pricing strategy interface
```csharp
public interface IPricingStrategy
{
    double CalculateFare(double distance, double baseFare);
    string GetStrategyName();
}
```

**Task 4.2:** Implement these strategies:
- `StandardPricing` - $0.50 per km
- `RushHourPricing` - $1.00 per km
- `MidnightPricing` - $0.75 per km

**Task 4.3:** Create `Ride` class
- Properties: `string RideId`, `Rider Rider`, `Driver Driver`, `double Distance`
- Field: `IPricingStrategy _pricingStrategy` (default to StandardPricing)
- Methods:
  - `void SetPricingStrategy(IPricingStrategy strategy)`
  - `double CalculateFare()` - uses current strategy

### ✅ Test:
```csharp
Ride ride = new Ride("RIDE-001", rider, driver, 10);

ride.SetPricingStrategy(new StandardPricing());
Console.WriteLine($"Standard: ${ride.CalculateFare()}");

ride.SetPricingStrategy(new RushHourPricing());
Console.WriteLine($"Rush Hour: ${ride.CalculateFare()}");
```

**Expected:** Different fares for same ride.

---

## 🎯 Phase 5: Adapter Pattern - External Payments

### 🤔 The Problem:
bKash uses different methods than our payment interface.

### 📝 Your Tasks:

**Task 5.1:** Create payment interface
```csharp
public interface IPaymentProcessor
{
    bool Pay(string paymentInfo, double amount);
    string GetPaymentMethod();
}
```

**Task 5.2:** Create external bKash gateway (given code)
```csharp
public class BkashPaymentGateway
{
    public string SendMoney(string phoneNumber, double amount)
    {
        Console.WriteLine($"bKash: Sending ${amount} to {phoneNumber}");
        return "TXN" + new Random().Next(1000, 9999);
    }
    
    public bool CheckStatus(string transactionId)
    {
        Console.WriteLine($"bKash: {transactionId} successful");
        return true;
    }
}
```

**Task 5.3:** Create `BkashPaymentAdapter`
- Implements `IPaymentProcessor`
- Uses `BkashPaymentGateway` internally
- Translates `Pay()` to `SendMoney()`

**Task 5.4:** Create `CreditCardProcessor`
- Implements `IPaymentProcessor`
- Directly processes card payments

### ✅ Test:
```csharp
IPaymentProcessor bkash = new BkashPaymentAdapter();
IPaymentProcessor card = new CreditCardProcessor();

bkash.Pay("01712345678", 100);
card.Pay("4111111111111111", 100);
```

**Expected:** Both work through same interface.

---

## 🎯 Phase 6: Observer Pattern - Notifications

### 🤔 The Problem:
When ride status changes, we need to notify the rider and driver.

### 📝 Your Tasks:

**Task 6.1:** Create observer interface
```csharp
public interface IRideObserver
{
    void Update(string rideId, string status);
}
```

**Task 6.2:** Create observer classes:

**RiderNotifier:**
```csharp
public class RiderNotifier : IRideObserver
{
    private string _riderName;
    
    public RiderNotifier(string riderName)
    {
        _riderName = riderName;
    }
    
    public void Update(string rideId, string status)
    {
        Console.WriteLine($"[SMS to {_riderName}] Your ride {rideId} is now: {status}");
    }
}
```

**DriverNotifier:**
```csharp
public class DriverNotifier : IRideObserver
{
    private string _driverName;
    
    public DriverNotifier(string driverName)
    {
        _driverName = driverName;
    }
    
    public void Update(string rideId, string status)
    {
        Console.WriteLine($"[App to Driver {_driverName}] Ride {rideId} status: {status}");
    }
}
```

**Task 6.3:** Update Ride class to be Observable
- Add a private list: `List<IRideObserver> _observers`
- Add method: `void AddObserver(IRideObserver observer)`
- Add method: `void SetStatus(string newStatus)`
- In `SetStatus()`, loop through all observers and call their `Update()` method

**Statuses to use:** Requested → Accepted → In Progress → Completed

### ✅ Test:
```csharp
Ride ride = new Ride("RIDE-001", rider, driver, 15);

// Add observers
ride.AddObserver(new RiderNotifier("Fatima"));
ride.AddObserver(new DriverNotifier("Ahmed"));

// Change status
ride.SetStatus("Accepted");
ride.SetStatus("In Progress");
ride.SetStatus("Completed");
```

**Expected Output:**
```
[SMS to Fatima] Your ride RIDE-001 is now: Accepted
[App to Driver Ahmed] Ride RIDE-001 status: Accepted
[SMS to Fatima] Your ride RIDE-001 is now: In Progress
[App to Driver Ahmed] Ride RIDE-001 status: In Progress
[SMS to Fatima] Your ride RIDE-001 is now: Completed
[App to Driver Ahmed] Ride RIDE-001 status: Completed
```

---

## 🎉 Final Integration

### Create a Console Application with Menu System

This console app should take user inputs from console and perform different operations that will test the design patterns.

Your `Program.cs` should look like this:

```csharp
using RideSharing.Users;
using RideSharing.Vehicles;
using RideSharing.Management;
using RideSharing.Rides;
using RideSharing.Payments;
using RideSharing.Observers;
using RideSharing.Pricing;

namespace RideSharing
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: your code here...
        }        
    }
}
```

---

## 📁 Project Structure & Namespaces

**IMPORTANT:** Organize your code into separate files and namespaces. Don't put everything in one 400-line file!

```
RideSharing/
├── Program.cs
├── Users/
│   ├── User.cs              (namespace RideSharing.Users)
│   ├── Rider.cs             (namespace RideSharing.Users)
│   └── Driver.cs            (namespace RideSharing.Users)
├── Vehicles/
│   ├── IVehicle.cs          (namespace RideSharing.Vehicles)
│   ├── Bike.cs              (namespace RideSharing.Vehicles)
│   ├── CNG.cs               (namespace RideSharing.Vehicles)
│   ├── Car.cs               (namespace RideSharing.Vehicles)
│   └── VehicleFactory.cs    (namespace RideSharing.Vehicles)
├── Management/
│   └── RideManager.cs       (namespace RideSharing.Management)
├── Rides/
│   └── Ride.cs              (namespace RideSharing.Rides)
├── Pricing/
│   ├── IPricingStrategy.cs  (namespace RideSharing.Pricing)
│   ├── StandardPricing.cs   (namespace RideSharing.Pricing)
│   ├── RushHourPricing.cs   (namespace RideSharing.Pricing)
│   └── MidnightPricing.cs   (namespace RideSharing.Pricing)
├── Payments/
│   ├── IPaymentProcessor.cs       (namespace RideSharing.Payments)
│   ├── BkashPaymentGateway.cs     (namespace RideSharing.Payments)
│   ├── BkashPaymentAdapter.cs     (namespace RideSharing.Payments)
│   └── CreditCardProcessor.cs     (namespace RideSharing.Payments)
└── Observers/
    ├── IRideObserver.cs     (namespace RideSharing.Observers)
    ├── RiderNotifier.cs     (namespace RideSharing.Observers)
    └── DriverNotifier.cs    (namespace RideSharing.Observers)
```

### Example of proper namespace usage:

**Users/User.cs:**
```csharp
namespace RideSharing.Users
{
    public abstract class User
    {
        // Implementation
    }
}
```

**Vehicles/VehicleFactory.cs:**
```csharp
namespace RideSharing.Vehicles
{
    public class VehicleFactory
    {
        // Implementation
    }
}
```

**Program.cs:**
```csharp
using RideSharing.Users;
using RideSharing.Vehicles;
using RideSharing.Management;
// ... other using statements

namespace RideSharing
{
    class Program
    {
        // Implementation
    }
}
```

---

## 📋 Additional Requirements

### OOP Principles to Apply:

1. **Encapsulation**: Use private fields, public properties
2. **Inheritance**: User → Rider/Driver hierarchy
3. **Polymorphism**: Override DisplayInfo() and GetRole()
4. **Abstraction**: Abstract User class, interfaces

### SOLID Principles:

1. **Single Responsibility**: Each class has one job
2. **Open/Closed**: Open for extension, closed for modification
3. **Liskov Substitution**: Rider/Driver usable through User reference
4. **Interface Segregation**: Focused interfaces (IVehicle, IPricingStrategy, etc.)
5. **Dependency Inversion**: Depend on abstractions (IPaymentProcessor)

### Code Quality:

- Clear naming conventions
- Proper error handling
- Consistent formatting
- Separate files for each class
- Proper namespace organization

---

## 💡 Tips

- Start with Phase 1, test thoroughly before Phase 2
- Don't overthink - each pattern solves ONE problem
- Draw class diagrams to visualize inheritance
- Test each pattern independently first
- Keep each file focused on one class
- Use meaningful namespace names

---

## 📦 Submission

Submit a zip file containing:
- All `.cs` files organized in folders matching the namespace structure
- `README.md` with:
  - How to run the application
  - Which patterns you used and where
  - Class diagram showing inheritance hierarchy
  - Any assumptions made

**Good luck! 🚀**
