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

    public async Task<IEnumerable<TestResult>> GetResultsForTestAsync(int testId)
    {
        return await _context.TestResults.Where(tr => tr.TestId == testId).ToListAsync();
    }

    public async Task<TestResult?> GetByIdAsync(long id)
    {
        return await _context.TestResults.FindAsync(id);
    }

    public async Task<TestResult?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .AsNoTracking()
            .FirstOrDefaultAsync(tr => tr.ResultId == id);
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.TestResults.FindAsync(id);
        if (entity != null)
        {
            _context.TestResults.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}