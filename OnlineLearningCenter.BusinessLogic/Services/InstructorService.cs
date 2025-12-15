using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using OnlineLearningCenter.DataAccess.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public InstructorService(IInstructorRepository instructorRepository, ICourseRepository courseRepository, IMapper mapper)
    {
        _instructorRepository = instructorRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<InstructorDto>> GetPaginatedInstructorsAsync(string? searchString, int pageNumber)
    {
        var (instructors, totalCount) = await _instructorRepository.GetPaginatedInstructorsAsync(searchString, pageNumber, PageSize);
        var dtos = _mapper.Map<List<InstructorDto>>(instructors);
        return new PaginatedList<InstructorDto>(dtos, totalCount, pageNumber, PageSize);
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

    public async Task<List<CourseDto>> DeleteInstructorAsync(int id)
    {
        var allCourses = await _courseRepository.GetAllAsync();
        var blockingCourses = allCourses.Where(c => c.InstructorId == id).ToList();

        if (blockingCourses.Any())
        {
            return _mapper.Map<List<CourseDto>>(blockingCourses);
        }

        await _instructorRepository.DeleteAsync(id);
        return new List<CourseDto>();
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