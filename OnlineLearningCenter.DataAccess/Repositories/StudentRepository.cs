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

    public IQueryable<Student> GetStudentsQueryable()
    {
        return _context.Students.AsNoTracking();
    }
}