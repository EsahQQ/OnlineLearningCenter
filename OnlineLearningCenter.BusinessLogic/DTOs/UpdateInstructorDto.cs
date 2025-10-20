using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class UpdateInstructorDto
{
    [Required]
    public int InstructorId { get; set; }

    [Required(ErrorMessage = "Необходимо указать ФИО преподавателя")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;
}