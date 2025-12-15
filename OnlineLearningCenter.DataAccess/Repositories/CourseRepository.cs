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

    public async Task<(List<Course> Items, int TotalCount)> GetPaginatedCoursesAsync(
        string? searchString,
        string? category,
        string? difficulty,
        int? instructorId,
        bool showOnlyActive,
        int pageNumber,
        int pageSize)
    {

        var query = _context.Courses.Include(c => c.Instructor).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(c => c.Title.Contains(searchString));
        }
        if (showOnlyActive)
        {
            query = query.Where(c => c.Status == "Активен");
        }
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(c => c.Category == category);
        }
        if (!string.IsNullOrEmpty(difficulty))
        {
            query = query.Where(c => c.Difficulty == difficulty);
        }
        if (instructorId.HasValue)
        {
            query = query.Where(c => c.InstructorId == instructorId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Title) 
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .AsNoTracking()
            .OrderBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<(int TotalStudents, int CompletedStudents, double AverageScore)> GetCourseAnalyticsDataAsync(int courseId)
    {
        var totalStudents = await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .CountAsync();

        var completedStudents = await _context.Enrollments
            .Where(e => e.CourseId == courseId && e.Progress >= 100)
            .CountAsync();

        var averageScoreQuery = _context.TestResults
            .Where(tr => tr.Test.Module.CourseId == courseId);

        var averageScore = await averageScoreQuery.AnyAsync()
            ? await averageScoreQuery.AverageAsync(tr => tr.Score)
            : 0.0;

        return (totalStudents, completedStudents, averageScore);
    }
}