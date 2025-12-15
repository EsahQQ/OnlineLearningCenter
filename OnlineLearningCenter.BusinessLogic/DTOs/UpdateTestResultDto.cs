using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class UpdateTestResultDto
{
    [Required]
    public long TestResultId { get; set; }
    [Required]
    [Range(0, 100, ErrorMessage = "Балл должен быть от 0 до 100")]
    public int Score { get; set; }
}