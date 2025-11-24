using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class UpdateModuleDto
{
    [Required]
    public int ModuleId { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Порядковый номер")]
    public int OrderNumber { get; set; }
    public string? Materials { get; set; }
    [Required]
    public int CourseId { get; set; }
}