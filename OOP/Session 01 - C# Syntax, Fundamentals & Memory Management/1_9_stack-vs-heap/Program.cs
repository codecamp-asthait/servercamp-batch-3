using System;

class StackVsHeapDemo
{
    static void Main()
    {
        Console.WriteLine("=== STACK vs HEAP MEMORY ===\n");

        // ============================================
        // EXAMPLE 1: Simple Memory Allocation
        // ============================================
        Console.WriteLine("--- Example 1: Where does data live? ---\n");

        int studentCount = 3;              // STACK: value type, local variable
        string courseName = "C# Bootcamp"; // STACK: reference | HEAP: "C# Bootcamp" object
        string courseName2 = courseName; // STACK: reference copy (both point to same HEAP string)
        courseName2 = "Java Bootcamp"; // STACK: reference updated to new HEAP string

        Student alice = new Student();     // STACK: reference | HEAP: Student object
        alice.Name = "Alice";              // HEAP: inside Student object
        alice.MathGrade = 85;              // HEAP: inside Student object (value type field in object)

        Console.WriteLine("Memory Layout:");
        Console.WriteLine("\nSTACK (Main method frame):");
        Console.WriteLine("  studentCount: 3");
        Console.WriteLine("  courseName: [ref] ──→ HEAP");
        Console.WriteLine("  alice: [ref] ──→ HEAP");

        Console.WriteLine("\nHEAP:");
        Console.WriteLine("  \"C# Bootcamp\" (string object)");
        Console.WriteLine("  Student object { Name: \"Alice\", MathGrade: 85 }");

        // ============================================
        // EXAMPLE 2: Method Calls and Stack Frames
        // ============================================
        Console.WriteLine("\n\n--- Example 2: Method calls create stack frames ---\n");

        ProcessStudent(alice, studentCount);

        Console.WriteLine("\n✅ Back in Main()");
        Console.WriteLine($"alice.MathGrade = {alice.MathGrade}");  // 90 (object was modified!)
        Console.WriteLine($"studentCount = {studentCount}");  // Still 3 (value wasn't modified)

        // ============================================
        // EXAMPLE 3: Multiple References to Same Object
        // ============================================
        Console.WriteLine("\n\n--- Example 3: Multiple references, one object ---\n");

        Student student1 = new Student();
        student1.Name = "Bob";
        student1.MathGrade = 80;

        Student student2 = student1;  // Both reference SAME object on heap

        Console.WriteLine("Memory after Student student2 = student1:");
        Console.WriteLine("\nSTACK:");
        Console.WriteLine("  student1: [ref] ──┐");
        Console.WriteLine("  student2: [ref] ──┴──→ HEAP: Student { Name: \"Bob\", MathGrade: 80 }");
        Console.WriteLine("\nOnly ONE object exists on the heap!");

        student2.MathGrade = 100;
        Console.WriteLine($"\nAfter student2.MathGrade = 100:");
        Console.WriteLine($"student1.MathGrade = {student1.MathGrade}");  // 100!
    }

    static void ProcessStudent(Student s, int count)
    {
        Console.WriteLine("Inside ProcessStudent():");
        Console.WriteLine("\nSTACK (NEW frame created):");
        Console.WriteLine("  s: [ref] ──→ HEAP (same Student object as alice)");
        Console.WriteLine("  count: 3 (independent copy)");

        s.MathGrade = 90;  // Modifies the HEAP object (alice will see this!)
        count = 5;         // Modifies local STACK copy (Main won't see this)

        Console.WriteLine($"\nModified s.MathGrade to {s.MathGrade}");
        Console.WriteLine($"Modified count to {count}");
    }
    // When method exits, this stack frame is destroyed!
    // But the heap object remains (alice still references it)
}

class Student
{
    public string Name;     // Reference type field (stores reference on heap)
    public int MathGrade;   // Value type field (stored inside object on heap)
}

/*
STACK vs HEAP: COMPLETE BREAKDOWN
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

STACK:
┌─────────────────────────────────────────────────────────┐
│ WHAT:      Fast, automatic memory (LIFO - Last In First Out) │
│ STORES:    • Local variables (value types)              │
│            • Method parameters                           │
│            • References to heap objects                  │
│ SIZE:      Small (~1MB per thread)                       │
│ SPEED:     Very fast (just move a pointer)              │
│ LIFETIME:  Exists only during method execution          │
│ CLEANUP:   Automatic when method returns                │
└─────────────────────────────────────────────────────────┘

HEAP:
┌─────────────────────────────────────────────────────────┐
│ WHAT:      Large, flexible memory pool                   │
│ STORES:    • Objects (instances of classes)             │
│            • Strings                                     │
│            • Arrays                                      │
│ SIZE:      Large (GBs available)                         │
│ SPEED:     Slower than stack                             │
│ LIFETIME:  Lives until no references point to it        │
│ CLEANUP:   Garbage Collector (automatic, but delayed)   │
└─────────────────────────────────────────────────────────┘

IMPORTANT RULES:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. Value types as LOCAL VARIABLES → Stack
   int x = 5;  // Lives on stack

2. Value types as FIELDS in a class → Heap (part of object)
   class Student {
       public int Grade;  // Lives on heap (inside Student object)
   }

3. Reference types → Reference on Stack, Object on Heap
   Student s = new Student();
   // s (reference) on stack → Student object on heap

4. Method parameters are COPIED:
   • Value type parameter → copies value to stack
   • Reference type parameter → copies reference to stack (both point to same heap object)

5. String is special:
   • Reference type (lives on heap)
   • But behaves like value type (immutable)

STACK FRAME VISUALIZATION:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

BEFORE ProcessStudent() call:
┌──────────────────┐
│ Main() frame     │
│ studentCount: 3  │
│ alice: [ref]────→│──→ HEAP: Student object
└──────────────────┘

DURING ProcessStudent() call:
┌──────────────────┐
│ProcessStudent()  │  ← New frame pushed
│ s: [ref]────────→│──→ HEAP: Same Student object
│ count: 3         │
├──────────────────┤
│ Main() frame     │
│ studentCount: 3  │
│ alice: [ref]────→│──→ HEAP: Student object
└──────────────────┘

AFTER ProcessStudent() returns:
┌──────────────────┐
│ Main() frame     │
│ studentCount: 3  │  ← unchanged
│ alice: [ref]────→│──→ HEAP: Student object (MathGrade now 90!)
└──────────────────┘
    ↑
    ProcessStudent frame destroyed!

WHY THIS MATTERS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ Understand why reference types can be modified in methods
✅ Understand why value types can't (unless passed by ref)
✅ Foundation for understanding garbage collection
✅ Explains performance characteristics
✅ Critical for avoiding memory-related bugs
*/
