using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
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
}