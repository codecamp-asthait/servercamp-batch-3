public delegate void Operation(int num1, int num2);

public class Calculator
{
    public static void Sum(int a, int b)
    {
        Console.WriteLine($"Summation is: {a + b}");   
    }

    public static void Subtraction(int a, int b)
    {
        Console.WriteLine($"Subtraction is: {a - b}");   
    }

    public static int Multiply(int a, int b)
    {
        return a * b;
    }
}

public delegate void Notify();

public class PaymentService
{
    public Notify? NotifyWithDelegate;
    public event Notify? NotifyWithEvent;

    public void ProcessWithDelegate()
    {
        Console.WriteLine("ProcessWithDelegate: ");
        Console.WriteLine("Processed Completed");
        NotifyWithDelegate?.Invoke();
        Console.WriteLine();
    }

    public void ProcessWithEvent()
    {
        Console.WriteLine("ProcessWithEvent: ");
        Console.WriteLine("Processed Completed");
        NotifyWithEvent?.Invoke();
    }
}