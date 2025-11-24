using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class TestService : ITestService
{
    private readonly ITestRepository _testRepository;
    private readonly IMapper _mapper;

    public TestService(ITestRepository testRepository, IMapper mapper)
    {
        _testRepository = testRepository;
        _mapper = mapper;
    }

    public async Task<TestDto> CreateTestAsync(CreateTestDto testDto)
    {
        var test = _mapper.Map<Test>(testDto);
        await _testRepository.AddAsync(test);
        return _mapper.Map<TestDto>(test);
    }

    public async Task DeleteTestAsync(int testId)
    {
        await _testRepository.DeleteAsync(testId);
    }

    public async Task<TestDto?> GetTestByIdAsync(int testId)
    {
        var test = await _testRepository.GetByIdAsync(testId);
        return _mapper.Map<TestDto>(test);
    }

    public async Task<IEnumerable<TestDto>> GetTestsByModuleIdAsync(int moduleId)
    {
        var allTests = await _testRepository.GetAllAsync();
        var moduleTests = allTests.Where(t => t.ModuleId == moduleId);
        return _mapper.Map<IEnumerable<TestDto>>(moduleTests);
    }

    public async Task UpdateTestAsync(UpdateTestDto testDto)
    {
        var existingTest = await _testRepository.GetByIdAsync(testDto.TestId);
        if (existingTest == null) throw new KeyNotFoundException("Тест не найден");

        _mapper.Map(testDto, existingTest);
        await _testRepository.UpdateAsync(existingTest);
    }
}