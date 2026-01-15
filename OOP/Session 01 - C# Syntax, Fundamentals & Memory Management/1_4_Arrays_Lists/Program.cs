// ============================================
// ARRAYS
// ============================================

Console.WriteLine("=== Arrays ===\n");

// Declare and initialize array
int[] numbers = new int[5];  // Fixed size array
numbers[0] = 10;
numbers[1] = 20;
numbers[2] = 30;
numbers[3] = 40;
numbers[4] = 50;

// Or initialize directly
int[] scores = { 85, 92, 78, 95, 88 };

int[] num = { 1, 2, 3 };

Console.WriteLine("Scores:");
for (int i = 0; i < scores.Length; i++)
{
    Console.WriteLine($"Score {i + 1}: {scores[i]}");
}

// Array properties
Console.WriteLine($"\nArray Length: {scores.Length}");

// ============================================
// LISTS (Dynamic size)
// ============================================

Console.WriteLine("\n=== Lists ===\n");

// Create a list
List<string> students = new List<string>();

// Add elements
students.Add("Alice");
students.Add("Bob");
students.Add("Charlie");
students.Add("Diana");

Console.WriteLine("Students:");
foreach (string student in students)
{
    Console.WriteLine($"- {student}");
}

Console.WriteLine($"\nTotal students: {students.Count}");

// List operations
students.Remove("Bob");  // Remove by value
Console.WriteLine("\nAfter removing Bob:");
foreach (string student in students)
{
    Console.WriteLine($"- {student}");
}

// Check if exists
bool hasAlice = students.Contains("Alice");
Console.WriteLine($"\nContains Alice: {hasAlice}");

// Insert at specific position
students.Insert(1, "Eve");
Console.WriteLine("\nAfter inserting Eve at position 1:");
foreach (string student in students)
{
    Console.WriteLine($"- {student}");
}
