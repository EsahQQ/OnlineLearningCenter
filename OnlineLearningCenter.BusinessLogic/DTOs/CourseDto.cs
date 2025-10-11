namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;
}