using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class TestResultRepository : GenericRepository<TestResult>, ITestResultRepository
{
    public TestResultRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<TestResult>> GetResultsByTestIdWithDetailsAsync(int testId)
    {
        return await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .Where(tr => tr.TestId == testId)
            .OrderByDescending(tr => tr.Score)
            .AsNoTracking()
            .ToListAsync();
    }
}