namespace OnlineLearningCenter.DataAccess.Entities;

public class Module
{
    public int ModuleId { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = null!;
    public int OrderNumber { get; set; }
    public string? Materials { get; set; }

    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}