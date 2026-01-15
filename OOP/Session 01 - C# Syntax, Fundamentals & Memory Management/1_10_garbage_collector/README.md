# Garbage Collection & Object Lifetime

## The Big Question

**"We create objects with `new`, but when are they destroyed?"**

In languages like C/C++, you must manually delete objects. In C#, the **Garbage Collector (GC)** does this automatically.

---

## What is Garbage Collection?

**Garbage Collection** is C#'s automatic memory management system:
- You create objects with `new`
- GC automatically destroys unreachable objects
- You DON'T manually delete objects
- It's automatic, but **non-deterministic** (you can't predict exactly when it runs)

---

## When is an Object "Garbage"?

An object becomes **garbage** when it's **unreachable** - when no references point to it anymore.

### Ways Objects Become Unreachable:

**1. Going out of scope:**
```csharp
void CreateStudent()
{
    Student s = new Student("Alice", "S001");
    s.DisplayInfo();
}  // ← s goes out of scope, object becomes unreachable
```

**2. Reference set to null:**
```csharp
Student s = new Student("Alice", "S001");
s = null;  // Object is now unreachable
```

**3. Reassignment:**
```csharp
Student s = new Student("Alice", "S001");
s = new Student("Bob", "S002");  // Alice's object becomes unreachable
```

**4. Multiple references - ALL must be gone:**
```csharp
Student alice = new Student("Alice", "S001");
Student aliceAlias = alice;  // Two references to same object

alice = null;       // Object still reachable via aliceAlias
aliceAlias = null;  // NOW object is unreachable
```

---

## How Garbage Collection Works

### Three Phases:

**Phase 1: Marking**
- GC scans all "roots" (variables on stack, static fields, CPU registers)
- Marks all reachable objects
- Follows references to mark indirectly reachable objects

**Phase 2: Sweeping**
- Any unmarked objects are considered garbage
- Memory is reclaimed

**Phase 3: Compacting (optional)**
- Heap is reorganized to reduce fragmentation
- Live objects moved together
- References updated to new addresses

### Visual Process:

```
BEFORE GC:
Stack           Heap
┌─────┐        ┌──────────┐
│ s1 ─┼───────→│ Alice    │ ← Reachable
├─────┤        ├──────────┤
│ s2 ─┼───────→│ Bob      │ ← Reachable
└─────┘        ├──────────┤
               │ Charlie  │ ← UNREACHABLE (no references!)
               └──────────┘

AFTER GC:
Stack           Heap
┌─────┐        ┌──────────┐
│ s1 ─┼───────→│ Alice    │
├─────┤        ├──────────┤
│ s2 ─┼───────→│ Bob      │
└─────┘        └──────────┘
                    ↑
               Charlie destroyed!
```

---

## Generations (Performance Optimization)

GC uses **3 generations** to optimize performance:

**The Generational Hypothesis**: "Most objects die young"

```
Gen 0: [●●●●●●●●●●] ← Collect frequently (cheap, small)
         ↓
Gen 1: [●●●]        ← Survivors promoted, collect occasionally
         ↓
Gen 2: [●]          ← Long-lived, collect rarely (expensive but infrequent)
```

**Benefits:**
- ✅ Fast: Most GC runs only check Gen 0 (small and quick)
- ✅ Efficient: Don't scan long-lived objects repeatedly
- ✅ Smart: Adapts to object lifetime patterns

---

## When Does GC Run?

GC runs **automatically** when:
1. **Gen 0 is full** (most common trigger)
2. **Low memory condition**
3. Explicit `GC.Collect()` call (NOT RECOMMENDED!)

⚠️ **You CANNOT control exactly when GC runs!**
- It's non-deterministic
- Timing is unpredictable
- This is by design - trust the GC

---

## Stack vs Heap Cleanup

### Stack Cleanup: IMMEDIATE
```csharp
void Method()
{
    int x = 10;  // Stack
    // ...
}  // ← x destroyed INSTANTLY when method ends
```
- Deterministic (predictable)
- Happens immediately
- No GC needed

### Heap Cleanup: DELAYED
```csharp
void Method()
{
    Student s = new Student("Alice", "S001");  // Heap
    // ...
}  // ← s reference destroyed, but object on heap remains
   //   until GC runs (non-deterministic)
```
- Non-deterministic (unpredictable)
- Happens eventually
- GC handles it automatically

---

## Examples

### Example 1: Simple Unreachable Object
```csharp
static void Main()
{
    CreateAndLoseStudent();
    
    Console.WriteLine("Student object is now unreachable");
    Console.WriteLine("GC will collect it... eventually");
}

static void CreateAndLoseStudent()
{
    Student temp = new Student("Temporary", "TEMP");
    temp.DisplayInfo();
    // Method ends → temp reference destroyed → object unreachable
}
```

### Example 2: Multiple References

```csharp
Student alice = new Student("Alice", "S001");
Student aliceRef = alice;  // Both point to same object

alice = null;      // Object still alive (aliceRef points to it)
aliceRef = null;   // NOW object is unreachable → eligible for GC
```

### Example 3: Reassignment

```csharp
Student s = new Student("Alice", "S001");
s = new Student("Bob", "S002");  // Alice's object unreachable
// Bob's object is now reachable via s
```

### Example 4: Loop Creates Garbage

```csharp
for (int i = 0; i < 1000; i++)
{
    Student temp = new Student($"Student{i}", $"S{i}");
    // temp goes out of scope each iteration
    // 1000 objects become unreachable (Gen 0 fills up → GC runs)
}
```

---

## IDisposable Pattern (Important!)

### The Problem

Some objects hold **unmanaged resources** that shouldn't wait for GC:
- File handles
- Database connections
- Network sockets
- Graphics handles

### The Solution: IDisposable + using

**Bad (manual cleanup):**
```csharp
FileStream file = new FileStream("data.txt");
try
{
    file.Write(...);
}
finally
{
    file.Dispose();  // Must remember to call!
}
```

**Good (automatic cleanup):**
```csharp
using (FileStream file = new FileStream("data.txt"))
{
    file.Write(...);
}  // ← Dispose() called automatically, even if exception!
```

**Modern C# (even better):**
```csharp
using FileStream file = new FileStream("data.txt");
file.Write(...);
// Dispose() called at end of scope
```

### Key Points:
- `using` statement ensures `Dispose()` is called
- Works even if exceptions occur
- Use for files, database connections, network resources
- Don't confuse with `using` for namespaces (different!)

---

## Important Rules

### ✅ DO:
- Let GC do its job automatically
- Set large/unused references to null if holding unnecessarily
- Use `using` statements for IDisposable objects
- Trust the GC - it's highly optimized !!!

### ❌ DON'T:
- Call `GC.Collect()` in production code (hurts performance!)
- Try to "help" the GC by manually managing memory
- Assume objects are destroyed immediately when unreachable
- Use finalizers (`~ClassName`) unless absolutely necessary

---

## Common Mistakes

### Mistake 1: Calling GC.Collect()
```csharp
// ❌ DON'T DO THIS
for (int i = 0; i < 1000; i++)
{
    Student s = new Student($"S{i}", $"ID{i}");
    GC.Collect();  // TERRIBLE! Kills performance
}

// ✅ DO THIS
for (int i = 0; i < 1000; i++)
{
    Student s = new Student($"S{i}", $"ID{i}");
    // Let GC decide when to collect
}
```

### Mistake 2: Memory Leaks in C#

**Yes, you CAN have memory leaks despite GC!**

```csharp
public class StudentManager
{
    public static List<Student> AllStudents = new List<Student>();
    
    public void AddStudent(Student s)
    {
        AllStudents.Add(s);  // ← s is now reachable forever!
    }
}

// Later:
Student temp = new Student("Alice", "S001");
manager.AddStudent(temp);
temp = null;  // ← Object is STILL REACHABLE via AllStudents!
```

**Common causes:**
- Event handlers not unsubscribed
- Static collections holding references
- Circular references (rare, GC handles most)

---

## Recap

### Step 1: Show the Question (1 min)
"We've been creating objects with `new`. But when do they get destroyed?"

### Step 2: Introduce GC (2 min)
- Automatic memory management
- No manual deletion needed (unlike C/C++)
- Objects destroyed when unreachable

### Step 3: Show Unreachability (3 min)
Live code the examples:
- Method scope
- Setting to null
- Reassignment

### Step 4: Explain How It Works (2 min)
- Mark, Sweep, Compact
- Use visual diagrams (draw on whiteboard)

### Step 5: Generations (2 min)
- Gen 0, 1, 2
- "Most objects die young"
- Quick visual

### Step 6: IDisposable (2 min)
- Some resources need immediate cleanup
- `using` statement demo
- File example

### Step 7: Rules (1 min)
- Trust the GC
- Don't call GC.Collect()
- Use `using` for resources

---

## Key Takeaways for Students

1. **You DON'T manually delete objects in C#**
2. **Objects are destroyed when UNREACHABLE** (no references point to them)
3. **GC runs automatically and non-deterministically**
4. **Stack cleanup is immediate, heap cleanup is delayed**
5. **Use `using` for resources** (files, connections, etc.)
6. **Don't call GC.Collect()** in production code
7. **Trust the GC** - it's highly optimized

---

## Questions to Ask Students

1. "When does an object become garbage?" 
   → When it's unreachable (no references)

2. "Does setting a reference to null immediately destroy the object?"
   → No, only makes it eligible for GC (destruction happens later)

3. "Can you have memory leaks in C# despite having GC?"
   → Yes! If objects remain reachable (static collections, event handlers)

4. "When should you use the `using` statement?"
   → For objects that implement IDisposable (files, connections, etc.)

5. "Should you call GC.Collect() in your code?"
   → No! Let GC decide when to run (except for specific edge cases)

---

## Additional Resources to Share

- **Microsoft Docs**: Fundamentals of garbage collection
- **Performance tip**: Object pooling for frequently created/destroyed objects
- **Advanced topic**: Weak references (for caches)
- **Tool**: Visual Studio's memory profiler to see GC in action
