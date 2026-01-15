# Introduction to C#

- C# (pronounced “C-Sharp”) is a modern, object-oriented programming language created by Microsoft.
- Runs on the .NET platform.
- Commonly used for desktop apps, web apps, games (Unity), and cloud services.
- Similar syntax to C++ and Java, so easy to learn if you’ve seen those.

“Think of C# as the language we use to talk to the .NET framework — together, they make powerful applications.”

## 1️⃣ The new C# style (C# 9 / .NET 6 and later)

When you create a new console project today (e.g., in Visual Studio or dotnet new console), you get this:

```csharp
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
```

👉 What’s happening?

This uses “top-level statements” — a simplified syntax introduced in C# 9.

The compiler automatically wraps your code inside a class Program and a static void Main() method behind the scenes.

You can say:

“C# now lets us skip the boilerplate — it’s like shorthand.
Behind the scenes, this code is still inside a class and a Main method.”

## 2️⃣ The classic C# style (before C# 9)

Before top-level statements existed, the same program looked like this:

```csharp
using System;

namespace HelloWorldApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```

We can also use file-scoped namespace which is modern to block-scoped namespace.

```csharp
using System;

namespace HelloWorldApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
```
