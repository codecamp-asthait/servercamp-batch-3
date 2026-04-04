public class TestService
{
    private readonly TestChildService _testChildService;

    public TestService(TestChildService testChildService)
    {
        _testChildService = testChildService;
    }

    public void Print()
    {
        Console.WriteLine("Hello from TestService");
        _testChildService.Print();
    }
}

public class TestChildService
{
    public void Print()
    {
        Console.WriteLine("Hello from TestChildService");
    }
}
