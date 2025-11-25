using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetStudentWithProgressDataAsync(int studentId)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Modules)
            .Include(s => s.TestResults)
                .ThenInclude(tr => tr.Test)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public IQueryable<(Student Student, double AverageScore)> GetStudentRankingsQueryable(int? courseId = null)
    {
        var query = _context.TestResults.AsQueryable();

        if (courseId.HasValue)
        {
            query = query.Where(tr => tr.Test.Module.CourseId == courseId.Value);
        }

        return query
            .GroupBy(tr => tr.Student)
            .Select(g => new { 
                Student = g.Key,
                AverageScore = g.Average(tr => tr.Score)
            })
            .OrderByDescending(r => r.AverageScore)
            .Select(r => new ValueTuple<Student, double>(r.Student, r.AverageScore)); 
    }

    public async Task<(List<Student> Items, int TotalCount)> GetPaginatedStudentsAsync(string? searchString, int pageNumber, int pageSize)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(s => s.FullName.Contains(searchString));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<(Student Student, double AverageScore)>> GetStudentRankingsAsync(int? courseId)
    {
        var query = _context.TestResults.AsQueryable();

        if (courseId.HasValue)
        {
            query = query.Where(tr => tr.Test.Module.CourseId == courseId.Value);
        }

        var rankingData = await query
            .Include(tr => tr.Student)
            .GroupBy(tr => tr.Student)
            .Select(g => new {
                Student = g.Key,
                AverageScore = g.Average(tr => tr.Score)
            })
            .OrderByDescending(r => r.AverageScore)
            .ToListAsync();

        return rankingData.Select(r => (r.Student, r.AverageScore)).ToList();
    }
}