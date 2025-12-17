using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

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

    public async Task<PaginatedList<CourseDto>> GetPaginatedCoursesAsync(
        string? searchString,
        string? category,
        string? difficulty,
        int? instructorId,
        bool showOnlyActive,
        int pageNumber)
    {
        var (courses, totalCount) = await _courseRepository.GetPaginatedCoursesAsync(
            searchString, category, difficulty, instructorId, showOnlyActive, pageNumber, PageSize);

        var dtos = _mapper.Map<List<CourseDto>>(courses);

        return new PaginatedList<CourseDto>(dtos, totalCount, pageNumber, PageSize);
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesForSelectListAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
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

    public async Task UpdateCourseAsync(int id, UpdateCourseDto courseDto)
    {
        var existingCourse = await _courseRepository.GetByIdAsync(id);
        if (existingCourse == null)
        {
            throw new KeyNotFoundException($"Курс с ID {id} не найден.");
        }
        _mapper.Map(courseDto, existingCourse);
        await _courseRepository.UpdateAsync(existingCourse);
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        return await _courseRepository.GetAllCategoriesAsync();
    }

    public async Task<IEnumerable<string>> GetAllDifficultiesAsync()
    {
        return await _courseRepository.GetAllDifficultiesAsync();
    }

    public async Task<CourseAnalyticsDto?> GetCourseAnalyticsAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return null;
        }

        var (totalStudents, completedStudents, averageScore) = await _courseRepository.GetCourseAnalyticsDataAsync(courseId);

        return new CourseAnalyticsDto
        {
            CourseId = course.CourseId,
            CourseTitle = course.Title,
            TotalStudentsEnrolled = totalStudents,
            StudentsCompleted = completedStudents,
            AverageScoreForCourse = averageScore
        };
    }
}