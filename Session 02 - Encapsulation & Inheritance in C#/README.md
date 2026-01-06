# C# Object-Oriented Programming - Study Notes

## 🎯 Core Concepts

### Classes and Objects
- **Class**: A blueprint or template that defines the structure and behavior of objects
- **Object**: An instance of a class (the actual thing created from the blueprint)

**Example:**
```csharp
// Class = Blueprint for a car
class Car { }

// Object = Actual car instance
Car myCar = new Car();
```

---

## 🏛️ The Four Pillars of OOP

### 1. Encapsulation
**Definition:** Bundle the behavior and data of an entity into a single class, with data protection

**Key Points:**
- Groups related data and methods together
- Protects data using access modifiers (private, protected)
- Provides controlled access through properties/methods

**Example:**
```csharp
class BankAccount
{
    private double balance;  // Protected data
    
    public void Deposit(double amount)  // Controlled access
    {
        if (amount > 0)
            balance += amount;
    }
}
```

### 2. Inheritance
**Definition:** Allows a class to inherit properties and methods from another class

**Purpose:** Solves the DRY (Don't Repeat Yourself) principle

**Key Points:**
- Creates an **IS-A** relationship
- Child class inherits from parent class
- Promotes code reusability

**Example:**
```csharp
class Person { }           // Parent class
class Student : Person { } // Student IS-A Person
```

### 3. Abstraction
**Definition:** Hides complex implementation details and shows only essential features

**Key Points:**
- Abstract classes are **ONE WAY** to achieve abstraction (not the only way)
- Can also be achieved through interfaces
- Shows WHAT an object does, not HOW it does it

**Example:**
```csharp
abstract class Vehicle
{
    public abstract void Start();  // What it does
    // HOW it starts is hidden in derived classes
}
```

### 4. Polymorphism
**Definition:** Ability of objects to take multiple forms

**Types:**
- **Compile-time:** Method overloading
- **Runtime:** Method overriding

---

## 🔐 Access Modifiers

| Modifier | Access Level | Usage |
|----------|-------------|-------|
| `public` | Accessible everywhere | Public API, methods |
| `private` | Only within the same class | Internal data, helper methods |
| `protected` | Within class and derived classes | Data for inheritance |
| `internal` | Within the same assembly | Internal implementation details |

---

## 📦 Properties vs Fields

### Why Properties are Preferred Over Fields?

**Fields:**
```csharp
public string name;  // Direct access, no control
```

**Properties:**
```csharp
private string name;
public string Name
{
    get { return name; }
    set 
    { 
        if (!string.IsNullOrEmpty(value))
            name = value; 
    }
}
```

**Advantages of Properties:**
1. ✅ **Validation:** Can validate data before setting
2. ✅ **Encapsulation:** Hide internal implementation
3. ✅ **Read-only/Write-only:** Control access level
4. ✅ **Computed values:** Can calculate on the fly
5. ✅ **Future-proof:** Can add logic later without breaking code

---

## 🔑 The `this` Keyword

### What is `this`?
**Definition:** A reference to the current instance of the class

### When is `this` Mandatory?

**1. Disambiguating Parameters and Fields:**
```csharp
class Person
{
    private string name;
    
    // MANDATORY: to distinguish parameter from field
    public Person(string name)
    {
        this.name = name;  // this.name = field, name = parameter
    }
}
```

**2. Calling Other Constructors:**
```csharp
class Person
{
    public Person() : this("Unknown") { }  // Calls constructor below
    public Person(string name) { }
}
```

**3. Passing Current Object as Parameter:**
```csharp
class Person
{
    public void Register()
    {
        DatabaseManager.Save(this);  // Pass current object
    }
}
```

### When is `this` Optional?
- When there's no naming conflict with parameters
- Most other cases where context is clear

---

## 🆚 Type Comparisons

### Class vs Record vs Record Struct

| Feature | `class` | `record` | `record struct` |
|---------|---------|----------|-----------------|
| **Type** | Reference | Reference | Value |
| **Equality** | Reference-based | Value-based | Value-based |
| **Mutability** | Mutable | Immutable by default | Mutable |
| **Memory** | Heap | Heap | Stack |
| **Use Case** | General OOP | Immutable data models | Lightweight data |

**Examples:**

```csharp
// CLASS - Reference equality
class PersonClass { public string Name; }
var p1 = new PersonClass { Name = "John" };
var p2 = new PersonClass { Name = "John" };
Console.WriteLine(p1 == p2);  // FALSE (different references)

// RECORD - Value equality
record PersonRecord(string Name);
var r1 = new PersonRecord("John");
var r2 = new PersonRecord("John");
Console.WriteLine(r1 == r2);  // TRUE (same values)

// RECORD STRUCT - Value equality
record struct PersonStruct(string Name);
var s1 = new PersonStruct("John");
var s2 = new PersonStruct("John");
Console.WriteLine(s1 == s2);  // TRUE (same values)
```

---

## 🔗 Inheritance vs Composition

### Inheritance (IS-A Relationship)

**When to use:** When there's a clear "is a type of" relationship

```csharp
class Animal { }
class Dog : Animal { }  // Dog IS-A Animal ✅
```

**Advantages:**
- Code reusability
- Polymorphism
- Clear hierarchies

**Disadvantages:**
- Tight coupling
- Fragile base class problem
- Can't change parent at runtime

### Composition (HAS-A Relationship)

**When to use:** When an object "has a" component or "uses a" service

```csharp
class Car
{
    private Engine engine;  // Car HAS-A Engine ✅
    
    public Car()
    {
        engine = new Engine();
    }
}
```

**Advantages:**
- Loose coupling
- More flexible
- Can change behavior at runtime
- Better testability

**Disadvantages:**
- More code to write
- More objects to manage

---

## 💡 Design Principles

### DRY Principle
**Don't Repeat Yourself**

**Bad:**
```csharp
class Teacher
{
    public void StayLate() { /* 10 lines of code */ }
}
class Student
{
    public void StayLate() { /* Same 10 lines of code */ }
}
```

**Good (Composition):**
```csharp
class LateSchoolManager
{
    public void StayLate() { /* 10 lines of code */ }
}

class Teacher
{
    private LateSchoolManager manager = new();
    public void StayLate() => manager.StayLate();
}
```

### Composition Over Inheritance
**Prefer composition when:**
- Behavior is shared across unrelated classes
- You need flexibility to change behavior
- Inheritance hierarchy becomes too deep
- Multiple classes need same functionality but aren't related

---

## 📝 TODO: Further Learning

- [ ] Deep dive into `internal` access modifier and assembly concepts
- [ ] Advanced differences between `class` and `record`
- [ ] Interfaces vs Abstract classes
- [ ] SOLID principles
- [ ] Design patterns (Factory, Strategy, etc.)
- [ ] When to use `sealed` keyword
- [ ] Virtual methods and method overriding

---

## 🎓 Quick Reference

**Key Takeaways:**
1. Use **properties** instead of public fields for better encapsulation
2. `this` is mandatory when parameter names match field names
3. **Inheritance** = "IS-A" relationship (tight coupling)
4. **Composition** = "HAS-A" relationship (loose coupling, preferred)
5. **Records** provide value-based equality automatically
6. Always follow **DRY** principle - don't repeat code
7. **Abstract classes** are one way to achieve abstraction, not the only way

---

**Remember:** Good OOP design balances all these concepts based on the specific problem you're solving!
