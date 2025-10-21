namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class StudentCourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal ModulesCompletedProgress { get; set; }
    public double AverageTestScore { get; set; }
}