// ===== Intro to Singleton (with singleton - single thread) =====

// Whiteboard teacherBoard = new Whiteboard();
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

    private string Content = "";

    // Private default constructor: Block creation of new instances from outside the class
    private Whiteboard()
    {
    }


    //  public way to get access to the one instance
    public static Whiteboard GetInstance()
    {
        if (_instance == null) // 1st call only enters this block of code
        {
            _instance = new Whiteboard();
        }

        return _instance; // 2nd, 3rd, .., nth call serves from here
    }

    public void Write(string text)
    {
        Content = Content + text + "\n";
    }

    public void View()
    {
        Console.WriteLine(Content);
    }
}
