using System;
using System.Collections.Generic;

namespace AssociationByDirection;

// ------------------------
// 1️⃣ Unidirectional Association
// Only one class knows about the other
// Example: Teacher → Student
// ------------------------
class Student
{
    public string Name { get; set; }
}

class Teacher
{
    public string Name { get; set; }

    public void Teach(Student student)
    {
        Console.WriteLine($"Teacher {Name} teaches {student.Name}");
    }
}

// ------------------------
// 2️⃣ Bidirectional Association
// Both classes know about each other
// Example: Student ↔ Course
// ------------------------
class Course
{
    public string Title { get; set; }
    public List<Student2> Students { get; set; } = new List<Student2>();
}

class Student2
{
    public string Name { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>();
}

// ------------------------
// 3️⃣ Self-association (Uni-directional)
// A class is associated with itself
// Example: Employee → Manager (an employee can have a manager)
// ------------------------
class Employee
{
    public string Name { get; set; }
    public Employee Manager { get; set; } // Self uni-directional
}

// ------------------------
// Program Entry
// ------------------------
// class Program
// {
    // static void Main(string[] args)
    // {
    //     // ----- Unidirectional -----
    //     Student s1 = new Student { Name = "Ali" };
    //     Teacher t1 = new Teacher { Name = "Mr. Rahman" };
    //     t1.Teach(s1); // Teacher knows Student, Student doesn't know Teacher
    //     Console.WriteLine();

    //     // ----- Bidirectional -----
    //     Student2 st1 = new Student2 { Name = "Naim" };
    //     Student2 st2 = new Student2 { Name = "Sara" };
    //     Course c1 = new Course { Title = "Math" };
    //     Course c2 = new Course { Title = "Physics" };

    //     // Assign courses to students and vice versa
    //     st1.Courses.Add(c1);
    //     st1.Courses.Add(c2);
    //     st2.Courses.Add(c1);

    //     c1.Students.Add(st1);
    //     c1.Students.Add(st2);
    //     c2.Students.Add(st1);

    //     Console.WriteLine($"Bidirectional: {st1.Name} enrolled in {st1.Courses.Count} courses");
    //     Console.WriteLine($"Bidirectional: {c1.Title} has {c1.Students.Count} students");
    //     Console.WriteLine();

    //     // ----- Self Uni-directional -----
    //     Employee e1 = new Employee { Name = "Alice" };
    //     Employee e2 = new Employee { Name = "Bob", Manager = e1 }; // Bob's manager is Alice
    //     Console.WriteLine($"Self Uni-directional: {e2.Name}'s manager is {e2.Manager.Name}");
    // }
// }

