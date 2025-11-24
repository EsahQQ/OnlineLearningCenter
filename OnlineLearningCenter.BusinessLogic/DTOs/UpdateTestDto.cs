using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class UpdateTestDto
{
    [Required]
    public int TestId { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public int ModuleId { get; set; }
}