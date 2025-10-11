namespace OnlineLearningCenter.DataAccess.Entities;

public class Course
{
    public int CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Difficulty { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int InstructorId { get; set; }

    public virtual Instructor Instructor { get; set; } = null!;
    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
}