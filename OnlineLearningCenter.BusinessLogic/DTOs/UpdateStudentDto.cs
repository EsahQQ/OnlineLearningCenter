using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class UpdateStudentDto
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}