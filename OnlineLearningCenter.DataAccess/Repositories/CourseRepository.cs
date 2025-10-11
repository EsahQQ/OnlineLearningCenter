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

    public async Task<IEnumerable<Course>> GetActiveCoursesFilteredAsync(string? category, string? difficulty, int? instructorId)
    {
        var query = _context.Courses
            .Include(c => c.Instructor)
            .Where(c => c.Status == "Активен")
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(c => c.Category == category);
        }

        if (!string.IsNullOrEmpty(difficulty))
        {
            query = query.Where(c => c.Difficulty == difficulty);
        }

        if (instructorId.HasValue && instructorId.Value > 0)
        {
            query = query.Where(c => c.InstructorId == instructorId.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Course?> GetCourseWithDetailsAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Modules)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == id);
    }
}