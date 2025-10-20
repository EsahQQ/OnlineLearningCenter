using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto)
    {
        var course = _mapper.Map<Course>(courseDto);
        await _courseRepository.AddAsync(course);

        return _mapper.Map<CourseDto>(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        await _courseRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CourseDto>> GetActiveCoursesAsync(string? category, string? difficulty, int? instructorId)
    {
        var courses = await _courseRepository.GetActiveCoursesFilteredAsync(category, difficulty, instructorId);

        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepository.GetCourseWithDetailsAsync(id);
        if (course == null)
        {
            return null;
        }

        return _mapper.Map<CourseDto>(course);
    }

    public async Task UpdateCourseAsync(int id, CreateCourseDto courseDto)
    {
        var existingCourse = await _courseRepository.GetByIdAsync(id);
        if (existingCourse == null)
        {
            throw new KeyNotFoundException($"Курс с ID {id} не найден.");
        }
        _mapper.Map(courseDto, existingCourse);
        await _courseRepository.UpdateAsync(existingCourse);
    }
}