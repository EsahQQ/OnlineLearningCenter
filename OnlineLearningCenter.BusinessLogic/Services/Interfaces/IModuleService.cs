using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(int courseId);
    Task<ModuleDto?> GetModuleByIdAsync(int moduleId);
    Task<ModuleDto> CreateModuleAsync(CreateModuleDto moduleDto);
    Task UpdateModuleAsync(UpdateModuleDto moduleDto);
    Task DeleteModuleAsync(int moduleId);
}