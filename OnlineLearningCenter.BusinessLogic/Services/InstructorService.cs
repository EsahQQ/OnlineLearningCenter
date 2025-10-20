using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IMapper _mapper;

    public InstructorService(IInstructorRepository instructorRepository, IMapper mapper)
    {
        _instructorRepository = instructorRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<InstructorDto>>(instructors);
    }
}