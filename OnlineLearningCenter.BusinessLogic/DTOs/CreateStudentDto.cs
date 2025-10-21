using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateStudentDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}