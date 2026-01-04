// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Method();

void Method()
{
    Student s = new Student("Alice", "S001");  // Heap
                                               // ...
                                               // ← s reference destroyed, but object on heap remains
                                               //   until GC runs (non-deterministic)
}
FileStream file = new FileStream("data.txt");
