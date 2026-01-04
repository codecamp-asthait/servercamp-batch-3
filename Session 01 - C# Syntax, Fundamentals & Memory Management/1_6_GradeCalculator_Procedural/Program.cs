using System;

class ProceduralGradeCalculator
{
    static void Main()
    {
        Console.WriteLine("=== PROCEDURAL APPROACH ===\n");

        // Problem: Data is scattered in separate variables
        string name1 = "Alice Johnson";
        string id1 = "S001";
        int math1 = 85, science1 = 90, english1 = 88;

        string name2 = "Bob Smith";
        string id2 = "S002";
        int math2 = 78, science2 = 82, english2 = 85;

        string name3 = "Charlie Brown";
        string id3 = "S003";
        int math3 = 92, science3 = 88, english3 = 90;

        // Functions are separate from data
        double avg1 = CalculateAverage(math1, science1, english1);
        double avg2 = CalculateAverage(math2, science2, english2);
        double avg3 = CalculateAverage(math3, science3, english3);

        string status1 = GetStatus(avg1);
        string status2 = GetStatus(avg2);
        string status3 = GetStatus(avg3);

        // Display each student (passing ALL data every time!)
        DisplayStudent(name1, id1, math1, science1, english1, avg1, status1);
        DisplayStudent(name2, id2, math2, science2, english2, avg2, status2);
        DisplayStudent(name3, id3, math3, science3, english3, avg3, status3);

        // Improve a grade - need to pass and return everything
        Console.WriteLine($"\n📚 {name3} studied Math hard!");
        math3 = ImproveGrade(math3, 10);
        avg3 = CalculateAverage(math3, science3, english3); // Recalculate!
        status3 = GetStatus(avg3); // Recalculate status!
        DisplayStudent(name3, id3, math3, science3, english3, avg3, status3);
    }

    // All these functions need data passed to them
    static double CalculateAverage(int math, int science, int english)
    {
        return (math + science + english) / 3.0;
    }

    static string GetStatus(double average)
    {
        return average >= 60 ? "PASS" : "FAIL";
    }

    static int ImproveGrade(int currentGrade, int points)
    {
        int newGrade = currentGrade + points;
        return newGrade > 100 ? 100 : newGrade;
    }

    static void DisplayStudent(string name, string id, int math, int science,
                               int english, double avg, string status)
    {
        Console.WriteLine($"\n{new string('─', 40)}");
        Console.WriteLine($"Student ID: {id}");
        Console.WriteLine($"Name: {name}");
        Console.WriteLine($"Math: {math}");
        Console.WriteLine($"Science: {science}");
        Console.WriteLine($"English: {english}");
        Console.WriteLine($"Average: {avg:F2}");
        Console.WriteLine($"Status: {status}");
        Console.WriteLine($"{new string('─', 40)}");
    }
}

/* 
PROBLEMS WITH THIS APPROACH:
❌ Data (name, grades) and behavior (calculate, display) are SEPARATED
❌ Must pass many parameters to every function (error-prone!)
❌ Easy to pass wrong data to wrong function (math1 with name2?)
❌ When we change a grade, must manually recalculate avg and status
❌ Adding a 4th student? Copy-paste 10+ lines of code!
❌ No logical grouping - "student" is just scattered variables
❌ Hard to maintain and scale

SOLUTION: Object-Oriented Programming!
✅ Group related data and behavior together in a CLASS
✅ Each student object "knows" its own data and can act on itself
✅ Changes are automatic and consistent
*/