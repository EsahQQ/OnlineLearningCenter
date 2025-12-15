using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ITestResultRepository : IGenericRepository<TestResult>
{
    Task<IEnumerable<TestResult>> GetResultsByTestIdWithDetailsAsync(int testId);
    Task<IEnumerable<TestResult>> GetResultsForTestAsync(int testId);
    Task<TestResult?> GetByIdAsync(long id); 
    Task<TestResult?> GetByIdWithDetailsAsync(long id);
    Task DeleteByIdAsync(long id);
}