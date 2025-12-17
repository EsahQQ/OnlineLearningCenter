using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces;

public interface ITestService
{
    Task<IEnumerable<TestDto>> GetTestsByModuleIdAsync(int moduleId);
    Task<TestDto?> GetTestByIdAsync(int testId);
    Task<TestDto> CreateTestAsync(CreateTestDto testDto);
    Task UpdateTestAsync(UpdateTestDto testDto);
    Task DeleteTestAsync(int testId);
}