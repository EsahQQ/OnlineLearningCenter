using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;

    public ModuleService(IModuleRepository moduleRepository, IMapper mapper)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
    }

    public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto moduleDto)
    {
        var module = _mapper.Map<Module>(moduleDto);
        await _moduleRepository.AddAsync(module);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task DeleteModuleAsync(int moduleId)
    {
        await _moduleRepository.DeleteAsync(moduleId);
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(int moduleId)
    {
        var module = await _moduleRepository.GetByIdAsync(moduleId);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(int courseId)
    {
        var allModules = await _moduleRepository.GetAllAsync();
        var courseModules = allModules.Where(m => m.CourseId == courseId).OrderBy(m => m.OrderNumber);
        return _mapper.Map<IEnumerable<ModuleDto>>(courseModules);
    }

    public async Task UpdateModuleAsync(UpdateModuleDto moduleDto)
    {
        var existingModule = await _moduleRepository.GetByIdAsync(moduleDto.ModuleId);
        if (existingModule == null) throw new KeyNotFoundException("Модуль не найден");

        _mapper.Map(moduleDto, existingModule);
        await _moduleRepository.UpdateAsync(existingModule);
    }
}