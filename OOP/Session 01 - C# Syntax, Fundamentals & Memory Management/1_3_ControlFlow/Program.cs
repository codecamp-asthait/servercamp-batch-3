// ============================================
// GRADE PRINTING USING IF-ELSE STATEMENTS
// ============================================

Console.WriteLine("=== IF-ELSE Examples ===\n");

int score = 85;

if (score >= 90)
{
    Console.WriteLine($"Score {score}: Grade A - Excellent!");
}
else if (score >= 80)
{
    Console.WriteLine($"Score {score}: Grade B - Very Good!");
}
else if (score >= 70)
{
    Console.WriteLine($"Score {score}: Grade C - Good!");
}
else if (score >= 60)
{
    Console.WriteLine($"Score {score}: Grade D - Pass");
}
else
{
    Console.WriteLine($"Score {score}: Grade F - Fail");
}

// Ternary operator (shorthand if-else)

string result = score >= 60 ? "PASS" : "FAIL";

Console.WriteLine($"Result: {result}");

// ============================================
// SWITCH STATEMENTS
// ============================================

Console.WriteLine("\n=== SWITCH Example ===\n");

int dayNumber = 3;
string dayName;

switch (dayNumber)
{
    case 1:
        dayName = "Monday";
        break;
    case 2:
        dayName = "Tuesday";
        break;
    case 3:
        dayName = "Wednesday";
        break;
    case 4:
        dayName = "Thursday";
        break;
    case 5:
        dayName = "Friday";
        break;
    case 6:
        dayName = "Saturday";
        break;
    case 7:
        dayName = "Sunday";
        break;
    default:
        dayName = "Invalid day";
        break;
}

Console.WriteLine($"Day {dayNumber} is {dayName}");

// ============================================
// FOR LOOP
// ============================================

Console.WriteLine("\n=== FOR Loop ===\n");

Console.WriteLine("Counting from 1 to 5:");

for (int i = 1; i <= 5; i++)
{
    Console.WriteLine($"Count: {i}");
}

// ============================================
// WHILE LOOP
// ============================================

Console.WriteLine("\n=== WHILE Loop ===\n");

int countdown = 5;

Console.WriteLine("Countdown:");

while (countdown > 0)
{
    Console.WriteLine(countdown);
    countdown--;
}

Console.WriteLine("Blast off!");

// ============================================
// DO-WHILE LOOP
// ============================================

Console.WriteLine("\n=== DO-WHILE Loop ===\n");

int number = 1;

do
{
    Console.WriteLine($"Number: {number}");
    number++;
} while (number <= 3);

// ============================================
// FOREACH LOOP (preview - will use with arrays)
// ============================================

Console.WriteLine("\n=== FOREACH Loop ===\n");

string[] fruits = { "Apple", "Banana", "Orange", "Mango" };

Console.WriteLine("Fruits in basket:");

foreach (string fruit in fruits)
{
    Console.WriteLine($"- {fruit}");
}
