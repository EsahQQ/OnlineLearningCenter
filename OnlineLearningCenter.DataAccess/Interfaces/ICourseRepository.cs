using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetCourseWithDetailsAsync(int id);
    Task<IEnumerable<string>> GetAllCategoriesAsync();
    Task<IEnumerable<string>> GetAllDifficultiesAsync();
    Task<(List<Course> Items, int TotalCount)> GetPaginatedCoursesAsync(
            string? searchString,
            string? category,
            string? difficulty,
            int? instructorId,
            bool showOnlyActive,
            int pageNumber,
            int pageSize);
    Task<List<Course>> GetAllCoursesAsync();
    Task<(int TotalStudents, int CompletedStudents, double AverageScore)> GetCourseAnalyticsDataAsync(int courseId);
}