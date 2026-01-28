# Builder Pattern: From Problem to Solution

## The Problem with Constructors

Let's say you're building a system to create user accounts. Here's what happens when you rely only on constructors:

```csharp
public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Problem 1: Too Many Constructor Parameters

```csharp
// Creating a user becomes a nightmare!
var user = new User(
    "john_doe",           // username
    "john@example.com",   // email
    "securePass123",      // password
    "John",               // firstName
    "Doe",                // lastName
    30,                   // age
    "+1234567890",        // phoneNumber
    "123 Main St",        // address
    "New York",           // city
    "USA",                // country
    false,                // isEmailVerified
    true,                 // isActive
    DateTime.Now          // createdAt
);

// What does 'false' mean? What does 'true' mean?
// The order is confusing and error-prone!
```

### Problem 2: Multiple Constructors (Constructor Overloading Hell)

```csharp
public class User
{
    // Basic constructor
    public User(string username, string email, string password) { }
    
    // With names
    public User(string username, string email, string password, 
                string firstName, string lastName) { }
    
    // With names and age
    public User(string username, string email, string password,
                string firstName, string lastName, int age) { }
    
    // With everything
    public User(string username, string email, string password,
                string firstName, string lastName, int age,
                string phone, string address, string city, string country) { }
    
    // This gets out of control quickly!
    // You end up with dozens of constructors for different combinations
}
```

## What If

What if we could

- create complex objects without worrying about constructors?
- decide object creation steps on the fly??? like a multi-step creation

## The Solution: Builder Pattern

Now let's fix all these problems with the Builder pattern:

```csharp
using System;

// First let's see how builder pattern makes our life simple, not worry about implementation.
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== BUILDER PATTERN DEMO ===\n");
        
        // Example 1: Minimal user (only required fields)
        var basicUser = new User.Builder()
            .WithUsername("jane_smith")
            .WithEmail("jane@example.com")
            .WithPassword("secure123")
            .Build();
        
        Console.WriteLine("Basic User:");
        Console.WriteLine(basicUser);
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        
        // Example 2: Complete user profile
        var completeUser = new User.Builder()
            .WithUsername("john_doe")
            .WithEmail("john@example.com")
            .WithPassword("securePass456")
            .WithName("John", "Doe")
            .WithAge(30)
            .WithPhoneNumber("+1-555-1234")
            .WithAddress("123 Main St", "New York", "USA")
            .MarkAsVerified()
            .MarkAsActive()
            .Build();
        
        Console.WriteLine("Complete User:");
        Console.WriteLine(completeUser);
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        
        // Example 3: Partial user (only some optional fields)
        var partialUser = new User.Builder()
            .WithUsername("bob_wilson")
            .WithEmail("bob@example.com")
            .WithPassword("pass789")
            .WithName("Bob", "Wilson")
            .WithAge(25)
            .MarkAsActive()
            .Build();
        
        Console.WriteLine("Partial User:");
        Console.WriteLine(partialUser);
        Console.WriteLine("\n" + new string('-', 50) + "\n");
    }
}

// Now let's see how Builder class helps building User like a multi-step process...
public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Private constructor - force use of builder
    private User() { }
    
    public override string ToString()
    {
        return $"User: {Username} ({Email})\n" +
               $"Name: {FirstName} {LastName}\n" +
               $"Age: {Age}\n" +
               $"Location: {City}, {Country}\n" +
               $"Phone: {PhoneNumber}\n" +
               $"Active: {IsActive}, Verified: {IsEmailVerified}";
    }
    
    // Nested Builder class
    public class Builder
    {
        private User user = new User();
        
        public Builder WithUsername(string username)
        {
            user.Username = username;
            return this;
        }
        
        public Builder WithEmail(string email)
        {
            user.Email = email;
            return this;
        }
        
        public Builder WithPassword(string password)
        {
            user.Password = password;
            return this;
        }
        
        public Builder WithName(string firstName, string lastName)
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            return this;
        }
        
        public Builder WithAge(int age)
        {
            user.Age = age;
            return this;
        }
        
        public Builder WithPhoneNumber(string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return this;
        }
        
        public Builder WithAddress(string address, string city, string country)
        {
            user.Address = address;
            user.City = city;
            user.Country = country;
            return this;
        }
        
        public Builder MarkAsVerified()
        {
            user.IsEmailVerified = true;
            return this;
        }
        
        public Builder MarkAsActive()
        {
            user.IsActive = true;
            return this;
        }
        
        public Builder MarkAsInactive()
        {
            user.IsActive = false;
            return this;
        }
        
        public User Build()
        {
            // Set defaults
            user.CreatedAt = DateTime.Now;
            
            // Validation before creating
            if (string.IsNullOrEmpty(user.Username))
                throw new InvalidOperationException("Username is required");
            
            if (string.IsNullOrEmpty(user.Email))
                throw new InvalidOperationException("Email is required");
            
            if (string.IsNullOrEmpty(user.Password))
                throw new InvalidOperationException("Password is required");
            
            return user;
        }
    }
}
```

## Benefits of the Builder Pattern

### ✅ **Readability**
```csharp
// BEFORE (Constructor): What do these parameters mean?
var user = new User("john", "john@mail.com", "pass", "John", "Doe", 30, null, null, "NYC", "USA");

// AFTER (Builder): Crystal clear!
var user = new User.Builder()
    .WithUsername("john")
    .WithEmail("john@mail.com")
    .WithPassword("pass")
    .WithName("John", "Doe")
    .WithAge(30)
    .WithAddress(null, "NYC", "USA")
    .Build();
```

### ✅ **Flexibility**
```csharp
// Easy to add only what you need
var user1 = new User.Builder()
    .WithUsername("alice")
    .WithEmail("alice@mail.com")
    .WithPassword("pass")
    .Build();

var user2 = new User.Builder()
    .WithUsername("bob")
    .WithEmail("bob@mail.com")
    .WithPassword("pass")
    .WithAge(25)
    .WithPhoneNumber("123-456")
    .Build();
```

### ✅ **Validation**
```csharp
// Validate before object creation in Build() method
public User Build()
{
    if (string.IsNullOrEmpty(user.Email))
        throw new InvalidOperationException("Email required");
    
    return user;
}
```
