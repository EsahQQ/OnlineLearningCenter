using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateInstructorDto
{
    [Required(ErrorMessage = "Необходимо указать ФИО преподавателя")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;
}