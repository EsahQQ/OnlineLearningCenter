using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateTestDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public int ModuleId { get; set; }
}