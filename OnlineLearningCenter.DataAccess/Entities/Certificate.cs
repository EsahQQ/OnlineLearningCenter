namespace OnlineLearningCenter.DataAccess.Entities;

public class Certificate
{
    public int CertificateId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateOnly IssueDate { get; set; }
    public string CertificateUrl { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}