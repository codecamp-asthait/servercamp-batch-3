public class Enrollment
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime EnrollmentAt { get; set; } = DateTime.UtcNow;
}
