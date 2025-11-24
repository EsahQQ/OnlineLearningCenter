using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private const int PageSize = 10;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentCourseProgressDto>> GetStudentProgressAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentWithProgressDataAsync(studentId);
            if (student == null || student.Enrollments == null)
            {
                return Enumerable.Empty<StudentCourseProgressDto>();
            }

            var progressList = new List<StudentCourseProgressDto>();

            foreach (var enrollment in student.Enrollments)
            {
                var course = enrollment.Course;
                if (course == null || course.Modules == null) continue;

                var courseModuleIds = course.Modules.Select(m => m.ModuleId).ToHashSet();
                var relevantTestResults = student.TestResults.Where(tr => tr.Test != null && courseModuleIds.Contains(tr.Test.ModuleId));
                double averageScore = relevantTestResults.Any() ? relevantTestResults.Average(tr => tr.Score) : 0;

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


        public async Task<PaginatedList<StudentDto>> GetPaginatedStudentsAsync(int pageNumber)
        {
            var query = _studentRepository.GetStudentsQueryable();

            var dtoQuery = _mapper.ProjectTo<StudentDto>(query);

            return await PaginatedList<StudentDto>.CreateAsync(dtoQuery.OrderBy(s => s.FullName), pageNumber, PageSize);
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

        public async Task<IEnumerable<StudentRankingDto>> GetStudentRankingsAsync(int? courseId = null)
        {
            var rankingData = await _studentRepository.GetStudentRankingsAsync(courseId);

            var rankingDto = rankingData.Select(r => new StudentRankingDto
            {
                StudentId = r.Student.StudentId,
                FullName = r.Student.FullName,
                AverageScore = r.AverageScore
            });

            return rankingDto;
        }
    }
}