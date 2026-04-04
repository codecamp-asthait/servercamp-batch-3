/*
Delegate:
- A delegate is like a **pointer to a method**.
- Can store **one or more methods**.
- Can be invoked directly from anywhere that has access.
- Example:

    public delegate void Notify(string message);

    Notify notifier = msg => Console.WriteLine(msg);
    notifier("Hello from delegate"); // Directly calls method


Event:
- An event is a **delegate with restricted access**.
- Only the class that declares it can **invoke it**.
- Others can only **subscribe (+=)** or **unsubscribe (-=)**.
- Example:

    public class Publisher
    {
        public event Action OnNotify;

        public void RaiseEvent()
        {
            OnNotify?.Invoke(); // Only this class can call
        }
    }

    var pub = new Publisher();
    pub.OnNotify += () => Console.WriteLine("Event fired!");
    pub.RaiseEvent(); // Triggers the event
*/

public class Event
{
    public static void Explanation()
    {
        Console.WriteLine("Event: ");
        var paymentService = new PaymentService();
        paymentService.NotifyWithDelegate += () => Console.WriteLine("Email Send from Multicast Delegate");
        paymentService.NotifyWithDelegate += () => Console.WriteLine("SMS Send from Multicast Delegate");
        paymentService.ProcessWithDelegate();

        paymentService.NotifyWithEvent += () => Console.WriteLine("Email Send from Event");
        paymentService.NotifyWithEvent += () => Console.WriteLine("SMS Send from Event");
        paymentService.ProcessWithEvent();
        Console.WriteLine();
    }
}