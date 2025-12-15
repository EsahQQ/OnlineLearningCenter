using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public class TestResultService : ITestResultService
{
    private readonly ITestResultRepository _resultRepository;
    private readonly IMapper _mapper;

    public TestResultService(ITestResultRepository resultRepository, IMapper mapper)
    {
        _resultRepository = resultRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestResultDto>> GetResultsByTestIdAsync(int testId)
    {
        var results = await _resultRepository.GetResultsByTestIdWithDetailsAsync(testId);
        return _mapper.Map<IEnumerable<TestResultDto>>(results);
    }

    public async Task CreateResultAsync(CreateTestResultDto dto)
    {
        var testResult = _mapper.Map<TestResult>(dto);
        await _resultRepository.AddAsync(testResult);
    }

    public async Task DeleteResultAsync(long resultId)
    {
        await _resultRepository.DeleteByIdAsync(resultId);
    }

    public async Task<TestResultDto?> GetResultByIdAsync(long resultId)
    {
        var result = await _resultRepository.GetByIdWithDetailsAsync(resultId);
        return _mapper.Map<TestResultDto>(result);
    }

    public async Task UpdateResultAsync(UpdateTestResultDto dto)
    {
        var existingResult = await _resultRepository.GetByIdAsync(dto.TestResultId);
        if (existingResult == null)
        {
            throw new KeyNotFoundException("Результат теста не найден.");
        }

        existingResult.Score = dto.Score;
        await _resultRepository.UpdateAsync(existingResult);
    }
}