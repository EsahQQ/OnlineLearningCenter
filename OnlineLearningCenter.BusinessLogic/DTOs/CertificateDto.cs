namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CertificateDto
{
    public int CertificateId { get; set; }
    public int StudentId { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string IssueDate { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
}