namespace HelperMethods;

public class Utility
{
    // Method with no return value
    public static void Greet(string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }

    // Method with return value
    public static int Add(int a, int b)
    {
        return a + b;
    }

    // Method with array parameter
    public static double CalculateAverage(int[] numbers)
    {
        int sum = 0;
        foreach (int num in numbers)
        {
            sum += num;
        }
        return (double)sum / numbers.Length;
    }

    // Method finding maximum
    public static int FindMax(int[] numbers)
    {
        int max = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
            {
                max = numbers[i];
            }
        }
        return max;
    }

    // TODO: learn at home: ref vs out in C#
    // Method with out parameters
    public static void Divide(int dividend, int divisor, out int quotient, out int remainder)
    {
        quotient = dividend / divisor;
        remainder = dividend % divisor;
    }
}


