using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetActiveCoursesAsync(string? category, string? difficulty, int? instructorId);
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto);
    Task UpdateCourseAsync(int id, CreateCourseDto courseDto); 
    Task DeleteCourseAsync(int id);
}