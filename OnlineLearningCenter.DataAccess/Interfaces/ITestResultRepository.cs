using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ITestResultRepository : IGenericRepository<TestResult>
{
    Task<(List<TestResult> Items, int TotalCount)> GetPaginatedResultsByTestIdAsync(int testId, int pageNumber, int pageSize);
    Task<IEnumerable<TestResult>> GetResultsForTestAsync(int testId);
    Task<TestResult?> GetByIdAsync(long id); 
    Task<TestResult?> GetByIdWithDetailsAsync(long id);
    Task DeleteByIdAsync(long id);
    Task<List<TestResult>> GetAllResultsForStudentAsync(int studentId);
}