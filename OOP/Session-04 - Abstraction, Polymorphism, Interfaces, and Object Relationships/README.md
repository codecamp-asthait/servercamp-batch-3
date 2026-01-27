# Payment Gateway Example

This project demonstrates a production-grade implementation of a Payment Gateway system in C#. It includes models, services, and a program entry point to simulate real-world payment processing using different payment processors (e.g., Stripe, PayPal).

---

## Features

### 1. **Encapsulation**
- Private fields and public properties with validation.
- Example: `Transaction` class encapsulates transaction details.

### 2. **Inheritance**
- Base and derived classes for extensibility.
- Example: Payment processors implement the `IPaymentProcessor` interface.

### 3. **Polymorphism**
- Compile-time polymorphism: Method overloading.
- Runtime polymorphism: Virtual methods and overriding.

### 4. **Abstraction**
- Abstracting payment processing logic using interfaces.
- Example: `IPaymentProcessor` interface for different payment gateways.

### 5. **Composition**
- Composition is used to manage relationships between objects.
- Example: `PaymentGateway` uses a `Transaction` object and a payment processor.

---

## Project Structure

### Models
- **`Transaction`**: Represents a payment transaction with fields for ID, amount, timestamp, and status.

### Services
- **`IPaymentProcessor`**: Interface for payment processors.
- **`StripePaymentProcessor`**: Concrete implementation for Stripe.
- **`PayPalPaymentProcessor`**: Concrete implementation for PayPal.

### Core
- **`PaymentGateway`**: Handles payment transactions using a specified payment processor.

### Program Entry
- **`Program`**: Entry point for the application. Demonstrates the usage of the payment gateway.

---

## How to Run

1. **Build the Project**:
   ```bash
   dotnet build
   ```

2. **Run the Project**:
   ```bash
   dotnet run
   ```

3. **Output**:
   - The program will simulate a payment transaction using the selected payment processor (Stripe or PayPal).

---

## Example Output
```
Welcome to the Payment Gateway!
Initiating transaction 123e4567-e89b-12d3-a456-426614174000...
Processing payment of $100.50 through Stripe...
Transaction 123e4567-e89b-12d3-a456-426614174000 completed successfully.
```

---

## Concepts Demonstrated

### Encapsulation
- Private fields with public getters and setters.
- Data validation in properties.

### Inheritance
- `IPaymentProcessor` interface implemented by `StripePaymentProcessor` and `PayPalPaymentProcessor`.

### Polymorphism
- Runtime polymorphism using method overriding.
- Compile-time polymorphism using method overloading.

### Abstraction
- Abstracting payment logic using the `IPaymentProcessor` interface.

### Composition
- `PaymentGateway` composes a `Transaction` and a payment processor.

---

## Future Enhancements
- Add support for more payment processors.
- Implement logging and error handling.
- Add unit tests for each component.

---
