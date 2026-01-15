namespace OOPGradeCalculatorApp;

// ============================================
// Student Class - Data + Behavior Together
// ============================================
class Student
{
    // FIELDS (Data) - What a student HAS
    public string Name;
    public string StudentId;
    public int MathGrade;
    public int ScienceGrade;
    public int EnglishGrade;

    // METHODS (Behavior) - What a student CAN DO

    // Calculate average grade
    public double CalculateAverage()
    {
        return (MathGrade + ScienceGrade + EnglishGrade) / 3.0;
    }

    // Get pass/fail status
    public string GetStatus()
    {
        double average = CalculateAverage();
        return average >= 60 ? "PASS" : "FAIL";
    }

    // Display student information
    public void DisplayInfo()
    {
        Console.WriteLine($"\n{new string('─', 40)}");
        Console.WriteLine($"Student ID: {StudentId}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Math: {MathGrade}");
        Console.WriteLine($"Science: {ScienceGrade}");
        Console.WriteLine($"English: {EnglishGrade}");
        Console.WriteLine($"Average: {CalculateAverage():F2}");
        Console.WriteLine($"Status: {GetStatus()}");
        Console.WriteLine($"{new string('─', 40)}");
    }

    // Improve a specific grade
    public void ImproveGrade(string subject, int points)
    {
        if (subject.ToLower() == "math")
        {
            MathGrade += points;
            if (MathGrade > 100) MathGrade = 100;
        }
        else if (subject.ToLower() == "science")
        {
            ScienceGrade += points;
            if (ScienceGrade > 100) ScienceGrade = 100;
        }
        else if (subject.ToLower() == "english")
        {
            EnglishGrade += points;
            if (EnglishGrade > 100) EnglishGrade = 100;
        }
        // No need to manually recalculate average or status!
        // DisplayInfo() will automatically use the new values
    }
}
