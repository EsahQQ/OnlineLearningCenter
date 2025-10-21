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
}