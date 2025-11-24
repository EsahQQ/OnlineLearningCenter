using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetCourseWithDetailsAsync(int id);
    Task<IEnumerable<string>> GetAllCategoriesAsync();
    Task<IEnumerable<string>> GetAllDifficultiesAsync();
    Task<Course?> GetCourseForAnalyticsAsync(int courseId);
    IQueryable<Course> GetCoursesQueryable();
}