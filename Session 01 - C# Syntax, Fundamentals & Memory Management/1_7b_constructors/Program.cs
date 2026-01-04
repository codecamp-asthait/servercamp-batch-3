using System;

class ConstructorsDemo
{
    static void Main()
    {
        Console.WriteLine("=== CONSTRUCTORS: Initializing Objects ===\n");

        // ============================================
        // THE PROBLEM (without constructors)
        // ============================================
        Console.WriteLine("--- The Problem: Too many steps! ---\n");

        Student s1 = new Student();  // What does this do?
        s1.Name = "Alice";
        s1.StudentId = "S001";
        s1.MathGrade = 85;
        s1.ScienceGrade = 90;
        s1.EnglishGrade = 88;
        // 6 lines just to create one student!
        // Easy to forget a field → bugs!

        Console.WriteLine("Created student with 6 separate lines (tedious!)\n");

        // ============================================
        // SOLUTION 1: Default Constructor
        // ============================================
        Console.WriteLine("\n--- Default Constructor ---\n");

        Student s2 = new Student();  // Calls default constructor
        s2.DisplayInfo();

        // ============================================
        // SOLUTION 2: Parameterized Constructor
        // ============================================
        Console.WriteLine("\n--- Parameterized Constructor ---\n");

        Student s3 = new Student("Bob Smith", "S002", 78, 82, 85);
        Student s4 = new Student("Charlie Brown", "S003");  // Only name and ID

        // ============================================
        // SOLUTION 4: Copy Constructor
        // ============================================
        Console.WriteLine("\n--- Copy Constructor ---\n");

        Student s5 = new Student(s3);  // Create a copy of Bob
        s5.DisplayInfo();

        Console.WriteLine("\n✅ Proving they're different objects:");
        s5.MathGrade = 100;
        Console.WriteLine($"s3 (original) MathGrade: {s3.MathGrade}");  // Still 78
        Console.WriteLine($"s5 (copy) MathGrade: {s5.MathGrade}");      // Now 100

        // ============================================
        // WHEN ARE CONSTRUCTORS CALLED?
        // ============================================
        Console.WriteLine("\n\n--- Constructor Execution Order ---\n");

        Console.WriteLine("Creating array of students:");
        Student[] students = new Student[3];
        students[0] = new Student("Diana", "S004", 88, 92, 85);
        students[1] = new Student("Eve", "S005");
        students[2] = new Student();

        Console.WriteLine("\n✅ Each 'new' keyword calls a constructor!");
    }
}

// ============================================
// Student Class with ALL Constructor Types
// ============================================
class Student
{
    // Fields
    public string Name;
    public string StudentId;
    public int MathGrade;
    public int ScienceGrade;
    public int EnglishGrade;

    // ──────────────────────────────────────────
    // 1. DEFAULT CONSTRUCTOR
    // ──────────────────────────────────────────
    // Called when: new Student()
    // Purpose: Set default/safe values
    public Student()
    {
        Name = "Unknown";
        StudentId = "UNASSIGNED";
        MathGrade = 0;
        ScienceGrade = 0;
        EnglishGrade = 0;

        Console.WriteLine("⚙️  Default constructor called");
    }

    // ──────────────────────────────────────────
    // 2. PARAMETERIZED CONSTRUCTOR
    // ──────────────────────────────────────────
    // Called when: new Student("Alice", "S001", 85, 90, 88)
    // Purpose: Initialize with specific values
    public Student(string name, string id, int math, int science, int english)
    {
        Name = name;
        StudentId = id;
        MathGrade = math;
        ScienceGrade = science;
        EnglishGrade = english;

        Console.WriteLine($"⚙️  Parameterized constructor called for {name}");
    }

    // ──────────────────────────────────────────
    // 3. PARTIAL CONSTRUCTOR (Constructor Overloading)
    // ──────────────────────────────────────────
    // Called when: new Student("Bob", "S002")
    // Purpose: Allow flexible initialization
    public Student(string name, string id)
    {
        Name = name;
        StudentId = id;
        MathGrade = 50;      // Default passing grade
        ScienceGrade = 50;
        EnglishGrade = 50;

        Console.WriteLine($"⚙️  Partial constructor called for {name}");
    }

    // ──────────────────────────────────────────
    // 4. COPY CONSTRUCTOR
    // ──────────────────────────────────────────
    // Called when: new Student(existingStudent)
    // Purpose: Create a NEW object with same values
    public Student(Student other)
    {
        Name = other.Name;
        StudentId = other.StudentId + "_COPY";
        MathGrade = other.MathGrade;
        ScienceGrade = other.ScienceGrade;
        EnglishGrade = other.EnglishGrade;

        Console.WriteLine($"⚙️  Copy constructor called (copying {other.Name})");
    }

    // ──────────────────────────────────────────
    // Methods
    // ──────────────────────────────────────────
    public double CalculateAverage()
    {
        return (MathGrade + ScienceGrade + EnglishGrade) / 3.0;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Student: {Name} ({StudentId})");
        Console.WriteLine($"Grades: Math={MathGrade}, Science={ScienceGrade}, English={EnglishGrade}");
        Console.WriteLine($"Average: {CalculateAverage():F2}");
    }
}

/*
CONSTRUCTORS: COMPLETE GUIDE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

WHAT IS A CONSTRUCTOR?
┌─────────────────────────────────────────────────────────┐
│ • Special method that runs when you create an object    │
│ • Same name as the class                                │
│ • NO return type (not even void)                        │
│ • Called automatically with 'new' keyword               │
│ • Purpose: Initialize object's fields to valid state    │
└─────────────────────────────────────────────────────────┘

CONSTRUCTOR TYPES:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. DEFAULT CONSTRUCTOR
   public Student() { ... }
   
   • Takes no parameters
   • If you DON'T write ANY constructor, C# provides this automatically
   • If you write ANY constructor, you must explicitly write default if you want it

2. PARAMETERIZED CONSTRUCTOR
   public Student(string name, int grade) { ... }
   
   • Takes parameters to initialize fields
   • Most common type
   • Makes object creation clean and safe

3. CONSTRUCTOR OVERLOADING
   public Student() { ... }
   public Student(string name) { ... }
   public Student(string name, int grade) { ... }
   
   • Multiple constructors with different parameters
   • Provides flexibility in how objects are created
   • Same rules as method overloading

4. COPY CONSTRUCTOR
   public Student(Student other) { ... }
   
   • Takes another object of same type
   • Creates a NEW object with copied values
   • Less common in C# (unlike C++)
   • Use when you need a deep copy

KEY RULES:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✅ Constructor name MUST match class name exactly
✅ No return type (not even void)
✅ Can be public, private, or protected
✅ Called exactly once per object (when created with 'new')
✅ Can call another constructor using : this(...)

❌ Cannot be called directly like a method
❌ Cannot return a value
❌ Cannot be static (but static constructors exist - advanced topic)

CONSTRUCTOR vs ASSIGNMENT:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

WITHOUT CONSTRUCTOR (6 lines):
    Student s = new Student();
    s.Name = "Alice";
    s.StudentId = "S001";
    s.MathGrade = 85;
    s.ScienceGrade = 90;
    s.EnglishGrade = 88;

WITH CONSTRUCTOR (1 line):
    Student s = new Student("Alice", "S001", 85, 90, 88);

BENEFITS:
✅ Less code
✅ Can't forget to initialize a field
✅ Can enforce rules (e.g., grade must be 0-100)
✅ Cleaner, more maintainable code

COPY CONSTRUCTOR vs ASSIGNMENT:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ASSIGNMENT (copies reference):
    Student s1 = new Student("Alice", "S001", 85, 90, 88);
    Student s2 = s1;  // Both point to SAME object
    s2.MathGrade = 100;
    // s1.MathGrade is also 100!

COPY CONSTRUCTOR (creates new object):
    Student s1 = new Student("Alice", "S001", 85, 90, 88);
    Student s2 = new Student(s1);  // Creates NEW object
    s2.MathGrade = 100;
    // s1.MathGrade is still 85 (independent objects)

CONSTRUCTOR CHAINING (Advanced):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

You can call one constructor from another using : this(...)

    public Student() : this("Unknown", "UNASSIGNED", 0, 0, 0)
    {
        // Calls the parameterized constructor first
        Console.WriteLine("Default constructor body");
    }

    public Student(string name, string id) : this(name, id, 50, 50, 50)
    {
        // Reuses the full constructor
    }

COMMON MISTAKES:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

❌ Writing return type:
    public void Student() { }  // This is a METHOD, not a constructor!

❌ Forgetting to write default constructor after writing parameterized:
    // If you write this:
    public Student(string name) { ... }
    
    // Then this no longer works:
    Student s = new Student();  // ERROR!

❌ Confusing copy constructor with assignment:
    Student s2 = s1;           // Assignment (reference copy)
    Student s2 = new Student(s1);  // Copy constructor (new object)

WHEN TO USE EACH TYPE:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DEFAULT:           When you want safe default values
PARAMETERIZED:     When you have all data at creation time (most common)
OVERLOADED:        When objects can be created in different ways
COPY:              When you need independent duplicates of objects
*/
