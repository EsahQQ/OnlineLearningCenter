using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public InstructorService(IInstructorRepository instructorRepository, IMapper mapper)
    {
        _instructorRepository = instructorRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<InstructorDto>> GetPaginatedInstructorsAsync(string? searchString, int pageNumber)
    {
        var query = _instructorRepository.GetInstructorsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(i => i.FullName.Contains(searchString));
        }

        var dtoQuery = _mapper.ProjectTo<InstructorDto>(query);
        return await PaginatedList<InstructorDto>.CreateAsync(dtoQuery.OrderBy(i => i.FullName), pageNumber, PageSize);
    }

    public async Task<IEnumerable<InstructorDto>> GetAllInstructorsForSelectListAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<InstructorDto>>(instructors);
    }

    public async Task<InstructorDto> CreateInstructorAsync(CreateInstructorDto instructorDto)
    {
        var instructor = _mapper.Map<Instructor>(instructorDto);
        await _instructorRepository.AddAsync(instructor);
        return _mapper.Map<InstructorDto>(instructor);
    }

    public async Task DeleteInstructorAsync(int id)
    {
        await _instructorRepository.DeleteAsync(id);
    }

    public async Task<InstructorDto?> GetInstructorByIdAsync(int id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);
        return _mapper.Map<InstructorDto>(instructor);
    }

    public async Task UpdateInstructorAsync(UpdateInstructorDto instructorDto)
    {
        var existingInstructor = await _instructorRepository.GetByIdAsync(instructorDto.InstructorId);
        if (existingInstructor == null)
        {
            throw new KeyNotFoundException($"Преподаватель с ID {instructorDto.InstructorId} не найден.");
        }
        _mapper.Map(instructorDto, existingInstructor);
        await _instructorRepository.UpdateAsync(existingInstructor);
    }
}