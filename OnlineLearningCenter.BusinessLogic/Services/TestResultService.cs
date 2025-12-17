using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public class TestResultService : ITestResultService
{
    private readonly ITestResultRepository _resultRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public TestResultService(ITestResultRepository resultRepository, IMapper mapper)
    {
        _resultRepository = resultRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TestResultDto>> GetPaginatedResultsByTestIdAsync(int testId, int pageNumber)
    {
        var (results, totalCount) = await _resultRepository.GetPaginatedResultsByTestIdAsync(testId, pageNumber, PageSize);
        var dtos = _mapper.Map<List<TestResultDto>>(results);
        return new PaginatedList<TestResultDto>(dtos, totalCount, pageNumber, PageSize);
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