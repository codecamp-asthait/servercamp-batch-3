public class Delegate
{
    public static void Explanation()
    {
        Console.WriteLine("Delegate: ");
        // Func → Returns a value, can take 0+ parameters
        Func<int, int, int> OperationFunc = Calculator.Multiply;

        // Action → Returns void, can take 0+ parameters
        Action<string> printMessage = message => Console.WriteLine(message);

        // Predicate → Returns bool, takes 1 parameter
        Predicate<int> isEven = (int value) => value % 2 == 0;

        // Multicast Delegate
        //  - A delegate can hold multiple methods using '+='
        //  - Invoking it calls all methods in order
        //  - If return type exists, the return of the last method is returned
        Operation operations = Calculator.Sum;
        operations += Calculator.Subtraction;
        operations.Invoke(20, 10);
        Console.WriteLine();
    }
}