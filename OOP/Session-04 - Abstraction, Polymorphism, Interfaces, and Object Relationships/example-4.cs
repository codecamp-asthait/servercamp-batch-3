using System;
using System.Collections.Generic;

namespace AssociationExamples;

// ------------------------
// 1:1 Association Example
// ------------------------
class Passport
{
    public string Number { get; set; }
}

class Person
{
    public string Name { get; set; }
    public Passport Passport { get; set; } // 1:1 association
}

// ------------------------
// 1:N Association Example
// ------------------------
class Student
{
    public string Name { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>(); // For N:N later
}

class Teacher
{
    public string Name { get; set; }
    public List<Student> Students { get; set; } = new List<Student>(); // 1:N association
}

// ------------------------
// N:N Association Example
// ------------------------
class Course
{
    public string Title { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();
}

// class Program
// {
//     static void Main(string[] args)
//     {
//         // --------- 1:1 ---------
//         Passport p1 = new Passport { Number = "P12345" };
//         Person person = new Person { Name = "Naim", Passport = p1 };
//         Console.WriteLine($"1:1 → {person.Name} has Passport {person.Passport.Number}");

//         // --------- 1:N ---------
//         Student s1 = new Student { Name = "Ali" };
//         Student s2 = new Student { Name = "Sara" };
//         Teacher t1 = new Teacher { Name = "Mr. Rahman (Sir)" };
//         t1.Students.Add(s1);
//         t1.Students.Add(s2);
//         Console.WriteLine($"1:N → {t1.Name} teaches {t1.Students.Count} students");

//         // --------- N:N ---------
//         Course c1 = new Course { Title = "Math" };
//         Course c2 = new Course { Title = "Physics" };
//         // Assign courses to students
//         s1.Courses.Add(c1);
//         s1.Courses.Add(c2);
//         s2.Courses.Add(c1); // Multiple students can take same course
//         Console.WriteLine($"N:N → {s1.Name} takes {s1.Courses.Count} courses");
//         Console.WriteLine($"N:N → {s2.Name} takes {s2.Courses.Count} courses");

//         c1.Students.Add(s1);
//         c1.Students.Add(s2);
//         c2.Students.Add(s1);
//         Console.WriteLine($"N:N → {c1.Title} has {c1.Students.Count} students");
//         Console.WriteLine($"N:N → {c2.Title} has {c2.Students.Count} students");
//         // ali niche 2 ta course --> Math, Physics
//         // sara niche 1 ta course --> Math
//         // Many to many
//         // One to many 
//         // 1 ta department --> multiple students ache 
//         // 1 ta student --> 1 ta department

//         // Many to Many
//         // 1 ta student --> multiple courses
//         // 1 ta course --> multiple students
//     }
// }


