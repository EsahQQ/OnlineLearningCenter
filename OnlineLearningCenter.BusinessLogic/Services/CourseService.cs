using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
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
        // 1. Вызываем новый метод репозитория
        var (courses, totalCount) = await _courseRepository.GetPaginatedCoursesAsync(
            searchString, category, difficulty, instructorId, showOnlyActive, pageNumber, PageSize);

        // 2. Мапим полученный список Entity в список DTO
        var dtos = _mapper.Map<List<CourseDto>>(courses);

        // 3. Создаем и возвращаем PaginatedList
        return new PaginatedList<CourseDto>(dtos, totalCount, pageNumber, PageSize);
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesForSelectListAsync()
    {
        // Используем новый, простой метод
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
        var course = await _courseRepository.GetCourseForAnalyticsAsync(courseId);

        if (course == null)
        {
            return null;
        }

        int totalStudents = course.Enrollments.Count;
        int completedStudents = course.Enrollments.Count(e => e.Progress >= 100);

        var allTestResults = course.Modules
            .SelectMany(m => m.Tests)
            .SelectMany(t => t.TestResults);

        double averageScore = 0;
        if (allTestResults.Any())
        {
            averageScore = allTestResults.Average(tr => tr.Score);
        }

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