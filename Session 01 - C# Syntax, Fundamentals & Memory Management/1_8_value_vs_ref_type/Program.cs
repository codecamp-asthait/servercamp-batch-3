using System;

class ValueVsReferenceDemo
{
    static void Main()
    {
        Console.WriteLine("=== VALUE TYPES vs REFERENCE TYPES ===\n");

        // ============================================
        // PART 1: VALUE TYPES
        // ============================================
        Console.WriteLine("--- VALUE TYPES (int, double, bool, char) ---\n");

        int score1 = 85;
        int score2 = score1;  // COPIES the actual value
        score2 = 95;  // Change score2
        Console.WriteLine($"score1 = {score1}");  // Still 85! (independent)
        Console.WriteLine($"score2 = {score2}");  // Now 95

        Console.WriteLine("\n✅ Value types: Copying creates INDEPENDENT copies\n");

        // ============================================
        // PART 2: REFERENCE TYPES
        // ============================================
        Console.WriteLine("\n--- REFERENCE TYPES (class, string, array) ---\n");

        Student alice = new Student();
        alice.Name = "Alice";
        alice.MathGrade = 85;

        Student aliceErBhai = alice;  // COPIES the reference (both point to same object!)
        aliceErBhai.MathGrade = 95;  // Change through aliceErBhai
        Console.WriteLine($"alice.MathGrade = {alice.MathGrade}");  // Also 95! (same object)
        Console.WriteLine($"aliceErBhai.MathGrade = {aliceErBhai.MathGrade}");

        Console.WriteLine("\n⚠️  Reference types: Both variables point to THE SAME OBJECT\n");

        // ============================================
        // PART 3: COMMON MISTAKE
        // ============================================
        Console.WriteLine("\n\n--- COMMON MISTAKE ---\n");

        Student bob = new Student();
        bob.Name = "Bob";
        bob.MathGrade = 80;

        Student temp = bob;
        temp.MathGrade = 100;  // Trying to "copy" and modify

        Console.WriteLine($"bob.MathGrade = {bob.MathGrade}");  // Oops! Also 100

        Console.WriteLine("\n❌ This is NOT a copy! Both variables reference the same object.");
        Console.WriteLine("✅ To create a real copy, we need a Copy Constructor (coming later!)");
    }
}

// Simple Student class for demo
class Student
{
    public string Name;
    public int MathGrade;
}

/*
KEY TAKEAWAYS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

VALUE TYPES:
• int, double, float, bool, char, struct, enum
• Store the ACTUAL VALUE
• Assignment (=) creates an INDEPENDENT COPY
• Example: int x = 5; int y = x; // y has its own copy of 5

REFERENCE TYPES:
• class, string, array, List<T>, any object
• Store a REFERENCE (memory address) to the object
• Assignment (=) copies the REFERENCE, not the object
• Example: Student s1 = new Student(); Student s2 = s1; // Both point to same object

WHY THIS MATTERS:
• Unexpected behavior if you don't understand this
• Critical for understanding method parameters (next topic!)
• Foundation for understanding memory (Stack vs Heap)
*/