namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class ModuleDto
{
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public string? Materials { get; set; }
    public int CourseId { get; set; }
}