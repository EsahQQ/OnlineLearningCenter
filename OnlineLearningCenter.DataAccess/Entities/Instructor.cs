namespace OnlineLearningCenter.DataAccess.Entities;

public class Instructor
{
    public int InstructorId { get; set; }
    public string FullName { get; set; } = null!;
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}