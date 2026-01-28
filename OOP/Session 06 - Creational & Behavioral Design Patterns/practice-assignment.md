# 📘 Design Pattern Practice Tasks

**Level:** Freshers
**Type:** Console Application
**Language:** Any (C# / Java)

---

## 🟦 Assignment 1: Singleton Pattern

### 🎯 Objective

Learn how multiple object creation attempts can still result in **one shared instance**.

---

### 🧩 Problem Statement

In a company, **10 different departments** try to create their own printer.
However, the company has **only one physical printer**, and all departments must use it.

---

### 🛠 Tasks

1. Create a class named `PrinterManager`
2. The class should:

   * Print text to the console
   * Keep track of total pages printed
3. In the program:

   * Attempt to create **10 printer objects**
4. Ensure:

   * All 10 references point to **the exact same instance**

5. Make the singleton thread-safe with double-checked locking

---

### 📝 Evaluation

✔ 10 objects attempted
✔ Only one instance exists
✔ Shared state works correctly
✔ (Advanced) Shared state works correctly when attempted from parallel threads

---

---

## 🟦 Assignment 2: Factory Pattern

### 🎯 Objective

Create objects **without directly using their class names**.

---

### 🧩 Problem Statement

A shop accepts multiple payment methods.
The shop should not know **which payment class** is being used.

---

### 🛠 Tasks

1. Create an interface `IPayment`

   * Method: `Pay(amount)`
2. Implement the interface:

   * `CashPayment` class
   * `CardPayment` class
   * `MobilePayment` class
3. Create a class `PaymentFactory`

   * Input: payment type (`string` or `enum`)
   * Output: correct `IPayment` object

---

### 📝 Evaluation

✔ Factory returns correct object
✔ Client code is decoupled from payment classes
✔ The shop will know payment is happening via IPayment interface, but not through exact which implementation.

---

---

## 🟦 Assignment 3: Strategy Pattern

### 🎯 Objective

Change behavior at runtime **without modifying the main class**.

---

### 🧩 Problem Statement

A shop applies different discounts depending on the situation.

---

### 🛠 Tasks

1. Create an interface `IDiscountStrategy`

   * Method: `ApplyDiscount(price)`
2. Implement:

   * `NoDiscount`
   * `StudentDiscount` (10%)
   * `FestivalDiscount` (20%)
3. Create a class `BillCalculator`

   * Accepts a discount strategy
   * Calculates final price

---
