namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RegistrationDate { get; set; } = string.Empty; 
}