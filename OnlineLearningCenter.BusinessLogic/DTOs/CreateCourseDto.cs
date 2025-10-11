using System.ComponentModel.DataAnnotations;

namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateCourseDto
{
    [Required(ErrorMessage = "Название курса обязательно")]
    [StringLength(200, ErrorMessage = "Название не может быть длиннее 200 символов")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Укажите уровень сложности")]
    public string Difficulty { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите категорию")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите статус")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "Необходимо выбрать преподавателя")]
    public int InstructorId { get; set; }
}