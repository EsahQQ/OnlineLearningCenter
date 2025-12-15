using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public interface ITestResultService
{
    Task<PaginatedList<TestResultDto>> GetPaginatedResultsByTestIdAsync(int testId, int pageNumber);
    Task<TestResultDto?> GetResultByIdAsync(long resultId); 
    Task CreateResultAsync(CreateTestResultDto dto);
    Task UpdateResultAsync(UpdateTestResultDto dto);
    Task DeleteResultAsync(long resultId);
}