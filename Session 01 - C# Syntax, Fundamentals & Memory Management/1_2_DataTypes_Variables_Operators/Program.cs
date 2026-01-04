
// ============================================
// VARIABLES & DATA TYPES
// ============================================

// Integer types
int age = 25; // 32bit int
long population = 7800000000; // 64bit int

// Floating-point types
float temperature = 36.6f; // Use 'f' suffix for float
double price = 99.99;
decimal salary = 5000.50m;  // Use 'm' suffix for decimal

// Character and String
char grade = 'A';
string name = "John Doe";
string multilinestr = """
asdasd
asdasd
asdasd
""";

// Boolean
bool isStudent = true;

// Display values
Console.WriteLine("=== Variable Examples ===");
Console.WriteLine($"Name: {name}");
Console.WriteLine($"Age: {age}");
Console.WriteLine($"Grade: {grade}");
Console.WriteLine($"Is Student: {isStudent}");
Console.WriteLine($"Salary: ${salary}");

// ============================================
// OPERATORS
// ============================================

Console.WriteLine("\n=== Operators ===");

// Arithmetic operators
int a = 10, b = 3;
Console.WriteLine($"Addition: {a} + {b} = {a + b}");
Console.WriteLine($"Subtraction: {a} - {b} = {a - b}");
Console.WriteLine($"Multiplication: {a} * {b} = {a * b}");
Console.WriteLine($"Division: {a} / {b} = {a / b}");
Console.WriteLine($"Modulus: {a} % {b} = {a % b}");

// Comparison operators
Console.WriteLine($"\n{a} > {b}: {a > b}");
Console.WriteLine($"{a} == {b}: {a == b}");
Console.WriteLine($"{a} != {b}: {a != b}");

// Logical operators
bool condition1 = true, condition2 = false;
Console.WriteLine($"\nAND: {condition1} && {condition2} = {condition1 && condition2}");
Console.WriteLine($"OR: {condition1} || {condition2} = {condition1 || condition2}");
Console.WriteLine($"NOT: !{condition1} = {!condition1}");

// Increment/Decrement
int counter = 5;
Console.WriteLine($"\nCounter: {counter}"); // Counter: 5
counter++;
Console.WriteLine($"After increment: {counter}"); // Counter 6
counter--;
Console.WriteLine($"After decrement: {counter}"); // Counter 5
