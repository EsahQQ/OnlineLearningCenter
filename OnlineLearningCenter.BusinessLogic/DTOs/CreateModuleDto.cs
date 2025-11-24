using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateModuleDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Порядковый номер")]
    public int OrderNumber { get; set; }
    public string? Materials { get; set; }
    [Required]
    public int CourseId { get; set; }
}