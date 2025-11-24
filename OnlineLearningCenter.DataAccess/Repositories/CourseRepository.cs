using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetCourseWithDetailsAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == id);
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        return await _context.Courses
            .Select(c => c.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllDifficultiesAsync()
    {
        return await _context.Courses
            .Select(c => c.Difficulty)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();
    }

    public async Task<Course?> GetCourseForAnalyticsAsync(int courseId)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Tests)
                    .ThenInclude(t => t.TestResults)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
    }

    public IQueryable<Course> GetCoursesQueryable()
    {
        return _context.Courses.Include(c => c.Instructor).AsNoTracking();
    }
}