using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public interface ITestResultService
{
    Task<IEnumerable<TestResultDto>> GetResultsByTestIdAsync(int testId);
}