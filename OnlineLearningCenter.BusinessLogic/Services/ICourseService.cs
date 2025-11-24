using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineLearningCenter.BusinessLogic.Helpers;

namespace OnlineLearningCenter.BusinessLogic.Services;

public interface ICourseService
{
    Task<PaginatedList<CourseDto>> GetPaginatedCoursesAsync(
        string? category, string? difficulty, int? instructorId, bool showOnlyActive, int pageNumber);
    Task<IEnumerable<CourseDto>> GetAllCoursesForSelectListAsync();
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto);
    Task UpdateCourseAsync(int id, UpdateCourseDto courseDto);
    Task DeleteCourseAsync(int id);

    Task<IEnumerable<string>> GetAllCategoriesAsync();
    Task<IEnumerable<string>> GetAllDifficultiesAsync();
    Task<CourseAnalyticsDto?> GetCourseAnalyticsAsync(int courseId);
}