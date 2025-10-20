using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.DataAccess.Entities;

public class TestResult
{
    [Key]
    public long ResultId { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public int Score { get; set; }
    public DateOnly CompletionDate { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Test Test { get; set; } = null!;
}