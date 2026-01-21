// ===== Intro to Singleton (with singleton - multi thread) =====

Whiteboard teacherBoard = Whiteboard.GetInstance();
teacherBoard.Write("Today's topic: Design Patterns");
teacherBoard.View();

Whiteboard student1Board = Whiteboard.GetInstance();
student1Board.Write("My notes");
student1Board.View();

Whiteboard student2Board = Whiteboard.GetInstance();
student2Board.Write("Question: What is Singleton?");
student2Board.View();

class Whiteboard
{
    private static Whiteboard? _instance;
    private static readonly object _lockObject = new object(); // Lock for thread safety

    private string Content = "";

    // Private default constructor: Block creation of new instances from outside the class
    private Whiteboard()
    {
    }

    public static Whiteboard GetInstance()
    {
        lock (_lockObject) // Only one thread can enter at a time
        {
            if (_instance == null) // 1st call only enters this block of code despite of parallel threads
            {
                _instance = new Whiteboard();
            }
        }

        return _instance; // 2nd, 3rd, .., nth call serves from here
    }

    public void Write(string text)
    {
        lock (_lockObject)
        {
            Content = Content + text + "\n";
        }
    }

    public void View()
    {
        Console.WriteLine(Content);
    }
}

// Homework: 
// what is object type in C#?
// study thread safety in c#
// VVI..
// write a code that simulates two students parallely calling Whiteboard.GetInstance() and Write() method parallely.
// and see the behavior when locking is used and when not used...
