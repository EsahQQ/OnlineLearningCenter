using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using OnlineLearningCenter.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITestRepository _testRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly ITestResultRepository _resultRepository;
        private readonly IMapper _mapper;
        private const int PageSize = 10;

        public StudentService(IStudentRepository studentRepository, ITestRepository testRepository, IModuleRepository moduleRepository, ITestResultRepository resultRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _testRepository = testRepository;
            _moduleRepository = moduleRepository;
            _resultRepository = resultRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentCourseProgressDto>> GetStudentProgressAsync(int studentId)
        {
            var enrollments = await _studentRepository.GetEnrollmentsWithDetailsAsync(studentId);
            var allTestResults = await _resultRepository.GetAllResultsForStudentAsync(studentId);

            if (enrollments == null || !enrollments.Any())
            {
                return Enumerable.Empty<StudentCourseProgressDto>();
            }

            var resultsGroupedByCourse = allTestResults
                .Where(tr => tr.Test?.Module != null)
                .GroupBy(tr => tr.Test.Module.CourseId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var progressList = new List<StudentCourseProgressDto>();

            foreach (var enrollment in enrollments)
            {
                var course = enrollment.Course;
                if (course == null) continue;

                double averageScore = 0;
                if (resultsGroupedByCourse.TryGetValue(course.CourseId, out var relevantTestResults))
                {
                    if (relevantTestResults.Any())
                    {
                        averageScore = relevantTestResults.Average(tr => tr.Score);
                    }
                }

                var progressDto = new StudentCourseProgressDto
                {
                    CourseId = course.CourseId,
                    CourseTitle = course.Title,
                    ModulesCompletedProgress = enrollment.Progress,
                    AverageTestScore = averageScore
                };
                progressList.Add(progressDto);
            }

            return progressList;
        }


        public async Task<PaginatedList<StudentDto>> GetPaginatedStudentsAsync(string? searchString, int pageNumber)
        {
            var (students, totalCount) = await _studentRepository.GetPaginatedStudentsAsync(searchString, pageNumber, PageSize);
            var dtos = _mapper.Map<List<StudentDto>>(students);
            return new PaginatedList<StudentDto>(dtos, totalCount, pageNumber, PageSize);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            student.RegistrationDate = DateOnly.FromDateTime(System.DateTime.Now);
            await _studentRepository.AddAsync(student);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task UpdateStudentAsync(UpdateStudentDto studentDto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(studentDto.StudentId);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Студент с ID {studentDto.StudentId} не найден.");
            }
            _mapper.Map(studentDto, existingStudent);
            await _studentRepository.UpdateAsync(existingStudent);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }

        public async Task<PaginatedList<StudentRankingDto>> GetStudentRankingsAsync(int? courseId, int pageNumber)
        {
            var rankings = await _studentRepository.GetStudentRankingsAsync(courseId);

            var dtos = rankings.Select(r => new StudentRankingDto
            {
                StudentId = r.Student.StudentId,
                FullName = r.Student.FullName,
                AverageScore = r.AverageScore
            }).ToList();

            var totalCount = dtos.Count;
            var items = dtos.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();

            return new PaginatedList<StudentRankingDto>(items, totalCount, pageNumber, PageSize);
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsAvailableForTestAsync(int testId)
        {
            var test = await _testRepository.GetByIdAsync(testId);
            if (test == null) return Enumerable.Empty<StudentDto>();
            var module = await _moduleRepository.GetByIdAsync(test.ModuleId);
            if (module == null) return Enumerable.Empty<StudentDto>();
            var courseId = module.CourseId;

            var enrolledStudents = await _studentRepository.GetStudentsEnrolledInCourseAsync(courseId);

            var studentsWhoPassed = (await _resultRepository.GetResultsForTestAsync(testId))
                                    .Select(r => r.StudentId).ToHashSet();

            var availableStudents = enrolledStudents.Where(s => !studentsWhoPassed.Contains(s.StudentId));

            return _mapper.Map<IEnumerable<StudentDto>>(availableStudents);
        }
    }
}