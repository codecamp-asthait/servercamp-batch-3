using System;

class ProceduralArrayApproach
{
    static void Main()
    {
        Console.WriteLine("=== PROCEDURAL APPROACH (WITH ARRAYS) ===\n");

        // Competitive programming style: Parallel Arrays
        // Can deal with thousand to millions students 
        string[] names = { "Alice Johnson", "Bob Smith", "Charlie Brown", "asdsad" };
        string[] ids = { "S001", "S002", "S003", "S004" };
        int[] mathGrades = { 85, 78, 55, 92 };
        int[] scienceGrades = { 90, 82, 88, 40 };
        int[] englishGrades = { 88, 85, 90, 33 };
        string[] bloodGroups = { "A+", "B-", "O+", "AB+" }; // Extra array for demonstration

        // Display all students (must pass ALL arrays!)
        DisplayAllStudents(names, ids, mathGrades, scienceGrades, englishGrades);

        // Improve Charlie's Math grade (index 2)
        Console.WriteLine("\n📚 Charlie studied Math hard!");
        mathGrades[2] = ImproveGrade(mathGrades[2], 10);

        // Must display again with all arrays
        DisplayStudent(2, names, ids, mathGrades, scienceGrades, englishGrades);
    }

    // Display all students
    static void DisplayAllStudents(string[] names, string[] ids,
                                   int[] math, int[] science, int[] english, string[] bloodGroups)
    {
        for (int i = 0; i < names.Length; i++)
        {
            DisplayStudent(i, names, ids, math, science, english, bloodGroups);
        }
    }

    // Display a single student at given index
    static void DisplayStudent(int index, string[] names, string[] ids,
                              int[] math, int[] science, int[] english, string[] bloodGroups = null)
    {
        double avg = CalculateAverage(math[index], science[index], english[index]);
        string status = GetStatus(avg);

        Console.WriteLine($"\n{new string('─', 40)}");
        Console.WriteLine($"Student ID: {ids[index]}");
        Console.WriteLine($"Name: {names[index]}");
        Console.WriteLine($"Math: {math[index]}");
        Console.WriteLine($"Science: {science[index]}");
        Console.WriteLine($"English: {english[index]}");
        Console.WriteLine($"Average: {avg:F2}");
        Console.WriteLine($"Status: {status}");
        Console.WriteLine($"{new string('─', 40)}");
        if (bloodGroups != null)
        {
            Console.WriteLine($"Blood Group: {bloodGroups[index]}");
        }
    }

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
}

/* 
PROBLEMS WITH PARALLEL ARRAYS APPROACH:
❌ Data scattered across 5 separate arrays (names, ids, math, science, english)
❌ Must keep ALL arrays synchronized - same length, same order!
❌ Must pass multiple arrays to every function (error-prone)
❌ Easy to make mistakes: what if names[i] doesn't match mathGrades[i]?
❌ Adding a new field (e.g., History grade)? Create another array everywhere!
❌ Deleting a student? Must remove from ALL 5 arrays at the same index
❌ Index-based access is fragile - off-by-one errors are common
❌ No logical grouping - "student" is spread across 5 different arrays
❌ Hard to pass "one student" to a function - must pass index + all arrays

REAL-WORLD NIGHTMARE SCENARIO:
    // Oops! Added to wrong index
    names[3] = "Diana";
    ids[2] = "S004";  // Now Diana's ID is at wrong position!
    // Data is now corrupted and hard to debug

SOLUTION: Object-Oriented Programming!
✅ One Student object contains ALL related data
✅ Can't get out of sync - data is bundled together
✅ Easy to pass around - just pass the object
✅ Adding fields? Just add to the class, not 5+ places
*/