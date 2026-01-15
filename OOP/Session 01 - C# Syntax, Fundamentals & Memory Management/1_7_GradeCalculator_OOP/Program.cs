using System;
using OOPGradeCalculatorApp;

// ============================================
// OOP APPROACH - The Solution
// ============================================
namespace Program;

class OOPGradeCalculator
{
    static void Main()
    {
        Console.WriteLine("=== OBJECT-ORIENTED APPROACH ===\n");

        // Create student objects - data grouped together!
        Student student1 = new Student();
        student1.Name = "Alice Johnson";
        student1.StudentId = "S001";
        student1.MathGrade = 85;
        student1.ScienceGrade = 90;
        student1.EnglishGrade = 88;

        Student student2 = new Student();
        student2.Name = "Bob Smith";
        student2.StudentId = "S002";
        student2.MathGrade = 78;
        student2.ScienceGrade = 82;
        student2.EnglishGrade = 85;

        Student student3 = new Student();
        student3.Name = "Charlie Brown";
        student3.StudentId = "S003";
        student3.MathGrade = 92;
        student3.ScienceGrade = 88;
        student3.EnglishGrade = 90;

        // Each object can act on itself!
        student1.DisplayInfo();
        student2.DisplayInfo();
        student3.DisplayInfo();

        // Improve a grade - much simpler!
        Console.WriteLine($"\n📚 Charlie studied Math hard!");
        student3.ImproveGrade("Math", 10);  // Object handles everything internally
        student3.DisplayInfo();              // Automatically shows updated values
    }
}


/* 
BENEFITS OF OOP APPROACH:
✅ Data and behavior are BUNDLED together in Student class
✅ Each object is self-contained and manages its own state
✅ Methods don't need parameters (they use the object's own fields)
✅ Changes are automatic - no manual recalculation needed
✅ Adding students is simple - just create new objects
✅ Logical grouping - "student" is now a real thing, not scattered variables
✅ Easy to maintain and scale
✅ Can't accidentally mix data between students

KEY CONCEPTS:
• CLASS: Blueprint/template for creating objects (Student)
• OBJECT: Actual instance created from the class (student1, student2)
• FIELDS: Data the object holds (Name, MathGrade, etc.)
• METHODS: Actions the object can perform (DisplayInfo, CalculateAverage)
• ENCAPSULATION: Bundling data + behavior together
*/