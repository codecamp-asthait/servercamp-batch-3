// ============================================================================
// RIDE-SHARING SYSTEM - Demonstrating OOP Concepts in C#
// ============================================================================
// This code demonstrates:
// - Inheritance (IS-A relationships)
// - Composition (HAS-A relationships)
// - Abstract classes
// - Properties and encapsulation
// - Composition over inheritance principle
// ============================================================================

// ============================================================================
// MAIN PROGRAM - Testing the System
// ============================================================================

// Create riders
Rider rider = new("John Doe", "123-456-7890", "john.doe@example.com", "123 Main St", "ABC123", "DL123456");
Rider rider2 = new("Alice Brown", "555-555-5555", "alice.brown@example.com", "789 Pine Rd", "XYZ789", "DL789012");

// Create passenger
Passenger passenger = new("Jane Smith", "098-765-4321", "jane.smith@example.com", "456 Oak Ave", 4.5, "O+");

// Display information
rider.DisplayInfo();
rider2.DisplayInfo();
passenger.DisplayInfo();

// Test equality (same data, different objects)
Rider rider3 = new("John Doe", "123-456-7890", "john.doe@example.com", "123 Main St", "ABC123", "DL123456");
Console.WriteLine($"Are rider and rider3 equal? {rider == rider3}"); // Shows false due to separate location in memory

// Test record struct - value-based equality
Cat cat1 = new() { Name = "Whiskers", Age = 3 };
Cat cat2 = new() { Name = "Whiskers", Age = 3 };
Console.WriteLine($"Are cat1 and cat2 equal? {cat1 == cat2}");  // True - records compare by value!

// Process payments
var payment = new Payment(rider, passenger, 25.0, "Credit Card");
var payment2 = new Payment(rider2, passenger, 15.0, "Bkash");
var paymentManager = new PaymentManager();

paymentManager.ProcessPayment(payment);
paymentManager.ProcessPayment(payment2);


// ============================================================================
// DOMAIN CLASSES
// ============================================================================

/// <summary>
/// Abstract base class representing a person in the ride-sharing system
/// Demonstrates: Inheritance, Encapsulation, Abstract classes
/// </summary>
abstract class Person
{
    // Constructors
    public Person()
    {
        ID = GenerateUniqueID();
    }

    public Person(string name, string mobileNo, string email, string currentLocation)
    {
        ID = GenerateUniqueID();
        Name = name;
        MobileNo = mobileNo;
        Email = email;
        CurrentLocation = currentLocation;
    }

    // Properties
    public string Name { get; protected set; }
    protected int ID { get; init; }  // Immutable after initialization
    protected string MobileNo { get; set; }
    protected string Email { get; set; }
    protected string CurrentLocation { get; set; }
    protected double Rating { get; set; }

    // Methods
    public int GenerateUniqueID()
    {
        Random random = new Random();
        return random.Next(1000, 9999);
    }
}


/// <summary>
/// Rider class - represents a driver in the system
/// Demonstrates: IS-A relationship (Rider IS-A Person)
/// </summary>
class Rider : Person
{
    public Rider(string name, string mobileNo, string email, string currentLocation,
                 string vehicleRegistrationNo, string driverLicenseNo)
        : base(name, mobileNo, email, currentLocation)
    {
        VehicleRegistrationNo = vehicleRegistrationNo;
        DriverLicenseNo = driverLicenseNo;
    }

    // Rider-specific properties
    public string VehicleRegistrationNo { get; set; }
    public string DriverLicenseNo { get; set; }

    // Methods
    public void DisplayInfo()
    {
        Console.WriteLine($"Rider: {Name} | ID: {ID} | Mobile: {MobileNo} | Email: {Email} | " +
                         $"Location: {CurrentLocation} | Rating: {Rating} | " +
                         $"Vehicle: {VehicleRegistrationNo} | License: {DriverLicenseNo}");
    }

    public void ChangeEmail(string newEmail)
    {
        Email = newEmail;
    }
}


/// <summary>
/// Passenger class - represents a customer in the system
/// Demonstrates: IS-A relationship (Passenger IS-A Person)
/// </summary>
class Passenger : Person
{
    public Passenger(string name, string mobileNo, string email, string currentLocation,
                     double rating, string bloodGroup)
        : base(name, mobileNo, email, currentLocation)
    {
        Rating = rating;
        BloodGroup = bloodGroup;
    }

    // Passenger-specific properties
    public string BloodGroup { get; set; }

    // Methods
    public void DisplayInfo()
    {
        Console.WriteLine($"Passenger: {Name} | ID: {ID} | Mobile: {MobileNo} | Email: {Email} | " +
                         $"Location: {CurrentLocation} | Rating: {Rating} | Blood Group: {BloodGroup}");
    }
}


// ============================================================================
// COMPOSITION OVER INHERITANCE EXAMPLE
// ============================================================================

/// <summary>
/// Payment class - represents a transaction in the system
/// Demonstrates: Composition over Inheritance (Payment HAS-A Rider and HAS-A Passenger)
/// </summary>
class Payment
{
    public Payment(Rider rider, Passenger passenger, double amount, string paymentMethod)
    {
        Rider = rider;
        Passenger = passenger;
        Amount = amount;
        PaymentMethod = paymentMethod;
    }

    // Properties - demonstrating composition
    public Rider Rider { get; set; }          // HAS-A Rider
    public int RiderID { get; set; }
    public Passenger Passenger { get; set; }  // HAS-A Passenger
    public int PassengerID { get; set; }
    public double Amount { get; set; }
    public string PaymentMethod { get; set; }

    // Methods
    public void DisplayPaymentInfo()
    {
        Console.WriteLine($"Payment of ${Amount} using {PaymentMethod} was sent to {Rider.Name}");
    }
}

/// <summary>
/// PaymentManager - Helper class for payment operations
/// Demonstrates: No composition, no inheritance - just helper methods
/// </summary>
class PaymentManager
{
    public void ProcessPayment(Payment payment)  // Method injection
    {
        // Could handle different payment methods:
        // - Bkash processing
        // - ATM booth processing
        // - Credit card processing
        Console.WriteLine($"Processing payment of ${payment.Amount} using {payment.PaymentMethod}");
    }

    public bool ValidatePayment()
    {
        // Validation logic would go here
        return true;
    }
}


// ============================================================================
// COMPOSITION SUCCEEDS WHEN INHERITANCE FAILS
// ============================================================================
// This section demonstrates why composition is often better than inheritance
// Problem: Both Teacher and BlackStudent need "StayLateInSchool" functionality
// In this problem, no way you can inherit from a common base class without
// creating unnecessary classes or duplicating code.
// Solution: Use composition with a helper class instead of duplicating code

class Human
{
    string Name;
}

class Teacher : Human
{
    public LateSchoolManager LateSchoolManager = new LateSchoolManager();

    public void StayLateInSchool()
    {
        LateSchoolManager.StaysLateInSchool();
    }
}

class Student : Human
{
    string Roll;
}

class BlackStudent : Student
{
    public LateSchoolManager LateSchoolManager = new LateSchoolManager();

    public void StayLateInSchool()
    {
        LateSchoolManager.StaysLateInSchool();
    }
}

class WhiteStudent : Student
{
    // Doesn't need to stay late, so no LateSchoolManager
}

/// <summary>
/// Helper class that encapsulates "stay late" behavior
/// This allows multiple classes to use the same functionality without inheritance
/// </summary>
class LateSchoolManager
{
    public void StaysLateInSchool()
    {
        // Shared logic for staying late at school
        // (Implementation details would go here)
    }
}


// ============================================================================
// NOTES:
// ============================================================================
// 1. ID property is 'init' only - immutable after construction
// 2. Each Person gets a unique random ID (1000-9999)
// 3. Payment uses composition to link Rider and Passenger
// 4. PaymentManager is a stateless helper class
// 5. Composition over inheritance prevents code duplication
// 6. Record structs (Cat) provide automatic value-based equality
//    - Regular classes compare by reference (rider == rider3 is false)
//    - Record structs compare by value (cat1 == cat2 is true if data matches)
// ============================================================================

/// <summary>
/// Cat record struct - demonstrates value-based equality
/// Two Cat instances with same data are considered equal
/// </summary>
record struct Cat
{
    public string Name { get; set; }
    public int Age { get; set; }
}
