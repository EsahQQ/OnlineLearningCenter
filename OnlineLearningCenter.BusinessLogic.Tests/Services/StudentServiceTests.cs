using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningCenter.BusinessLogic.Services;

using OnlineLearningCenter.DataAccess.Interfaces;

using Entities = OnlineLearningCenter.DataAccess.Entities;


namespace OnlineLearningCenter.BusinessLogic.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly IMapper _mapper; 
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BusinessLogic.Mappings.MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _studentService = new StudentService(_mockStudentRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetStudentProgressAsync_ShouldCalculateProgressAndScoreCorrectly()
        {
            // Arrange 
            var studentId = 1;

            var student = new Entities.Student
            {
                StudentId = studentId,
                Enrollments = new List<Entities.Enrollment>
                {
                    new Entities.Enrollment
                    {
                        Progress = 50,
                        Course = new Entities.Course
                        {
                            CourseId = 10, Title = "Курс по C#",
                            Modules = new List<Entities.Module>
                            {
                                new Entities.Module { ModuleId = 101 },
                                new Entities.Module { ModuleId = 102 }
                            }
                        }
                    }
                },
                TestResults = new List<Entities.TestResult>
                {
                    new Entities.TestResult { Score = 80, Test = new Entities.Test { ModuleId = 101 } },
                    new Entities.TestResult { Score = 90, Test = new Entities.Test { ModuleId = 101 } },
                    new Entities.TestResult { Score = 100, Test = new Entities.Test { ModuleId = 999 } }
                }
            };

            _mockStudentRepository.Setup(repo => repo.GetStudentWithProgressDataAsync(studentId)).ReturnsAsync(student);

            // Act
            var result = await _studentService.GetStudentProgressAsync(studentId);

            // Assert 
            result.Should().NotBeNull();
            result.Should().HaveCount(1); 

            var courseProgress = result.First();
            courseProgress.CourseTitle.Should().Be("Курс по C#");
            courseProgress.ModulesCompletedProgress.Should().Be(50);

            courseProgress.AverageTestScore.Should().Be(85);
        }
    }
}