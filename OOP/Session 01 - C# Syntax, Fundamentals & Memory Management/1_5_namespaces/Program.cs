using HelperMethods;

// ============================================
// METHODS
// ============================================

Console.WriteLine("\n=== Methods ===\n");

// Call methods
Utility.Greet("John");

int sum = Utility.Add(15, 25);
Console.WriteLine($"Sum: {sum}");

// int[] scores = { 85, 92, 78, 95, 88 };
// double average = Utility.CalculateAverage(scores);
// Console.WriteLine($"Average score: {average:F2}");

// int max = Utility.FindMax(scores);
// Console.WriteLine($"Highest score: {max}");

// // Method with out parameter
// int quotient, remainder;
// Utility.Divide(17, 5, out quotient, out remainder);
// Console.WriteLine($"\n17 ÷ 5 = {quotient} remainder {remainder}");

// Learn at home: ref vs out

