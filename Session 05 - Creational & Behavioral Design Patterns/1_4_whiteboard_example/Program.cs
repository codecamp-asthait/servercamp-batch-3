// ===== Intro to Singleton (with singleton - multi thread - double checked locking) =====

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
    // The ONE and ONLY instance
    private static Whiteboard? _instance;
    private static readonly object _lockObject = new object(); // Lock for thread safety

    private string Content = "";

    // Private default constructor: Block creation of new instances from outside the class
    private Whiteboard()
    {
    }

    // Double-checked locking (not mandatory)
    public static Whiteboard GetInstance()
    {
        // you can comment out this line & only check instance == null once after locking, will still work.
        // its not right or wrong, it about 50% optimized or 100% optimized
        if (_instance == null) // more efficient, lock only when needed!
        {
            lock (_lockObject) // Only one thread can enter at a time
            {
                if (_instance == null) // 1st call only enters this block of code
                {
                    _instance = new Whiteboard();
                }
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
