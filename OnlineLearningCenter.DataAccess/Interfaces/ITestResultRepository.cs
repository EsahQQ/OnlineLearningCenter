using OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ITestResultRepository : IGenericRepository<TestResult> 
{
    Task<IEnumerable<TestResult>> GetResultsByTestIdWithDetailsAsync(int testId);
}