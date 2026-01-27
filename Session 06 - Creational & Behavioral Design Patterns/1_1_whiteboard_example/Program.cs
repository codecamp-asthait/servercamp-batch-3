// ===== Intro to Singleton (without singleton) =====

Whiteboard teacherBoard = new Whiteboard();
teacherBoard.Write("Today's topic: Design Patterns");

Whiteboard student1Board = new Whiteboard();
student1Board.Write("My notes");

Whiteboard student2Board = new Whiteboard();
student2Board.Write("Question: What is Singleton?");

// Now let's see what each person sees:
// teacherBoard.View();
// student1Board.View();
student2Board.View();

// Requirement:
// We want all teachers & students to see the common board.
// student2Board.View() should print:
//
// Today's topic: Design Patterns
// My notes
// Question: What is Singleton?

class Whiteboard
{
    // Homework: learn to replace Write() Read() method with get; set;
    private string Content = "";

    public void Write(string text)
    {
        Content = Content + text + "\n";
    }

    public void View()
    {
        Console.WriteLine(Content);
    }
}
