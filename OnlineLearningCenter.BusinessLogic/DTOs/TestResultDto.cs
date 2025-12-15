namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class TestResultDto
{
    public long TestResultId { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public int TestId { get; set; }
    public int Score { get; set; }
    public string CompletionDate { get; set; } = string.Empty;
}