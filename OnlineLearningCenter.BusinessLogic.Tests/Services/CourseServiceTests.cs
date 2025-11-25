using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using Entities = OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.BusinessLogic.Tests.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockMapper = new Mock<IMapper>();
            _courseService = new CourseService(_mockCourseRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourseDto_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var course = new Entities.Course { CourseId = courseId, Title = "Test Course", Instructor = new Entities.Instructor { FullName = "Test Instructor" } };
            var courseDto = new CourseDto { CourseId = courseId, Title = "Test Course", InstructorName = "Test Instructor" };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId)).ReturnsAsync(course);
            _mockMapper.Setup(mapper => mapper.Map<CourseDto>(course)).Returns(courseDto);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CourseDto>();
            result.Should().BeEquivalentTo(courseDto);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = 99;
            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId)).ReturnsAsync((Entities.Course)null); // Уточняем тип

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldCallAddAsyncOnRepository_AndReturnMappedDto()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto { Title = "New Course" };
            var course = new Entities.Course { CourseId = 1, Title = "New Course" };
            var courseDto = new CourseDto { CourseId = 1, Title = "New Course" };

            _mockMapper.Setup(m => m.Map<Entities.Course>(createCourseDto)).Returns(course);
            _mockMapper.Setup(m => m.Map<CourseDto>(course)).Returns(courseDto);

            // Act
            var result = await _courseService.CreateCourseAsync(createCourseDto);

            // Assert
            result.Should().BeEquivalentTo(courseDto);
            _mockCourseRepository.Verify(repo => repo.AddAsync(course), Times.Once);
        }

        [Fact]
        public async Task GetCourseAnalyticsAsync_ShouldCalculateAnalyticsCorrectly()
        {
            // Arrange
            var courseId = 1;
            var course = new Entities.Course
            {
                CourseId = courseId,
                Title = "Analytics Test Course",
                Enrollments = new List<Entities.Enrollment>
                {
                    new Entities.Enrollment { Progress = 100 },
                    new Entities.Enrollment { Progress = 50 },
                    new Entities.Enrollment { Progress = 100 }
                },
                Modules = new List<Entities.Module>
                {
                    new Entities.Module
                    {
                        Tests = new List<Entities.Test>
                        {
                            new Entities.Test
                            {
                                TestResults = new List<Entities.TestResult> { new Entities.TestResult { Score = 80 }, new Entities.TestResult { Score = 90 } }
                            }
                        }
                    },
                    new Entities.Module
                    {
                        Tests = new List<Entities.Test>
                        {
                            new Entities.Test
                            {
                                TestResults = new List<Entities.TestResult> { new Entities.TestResult { Score = 70 } }
                            }
                        }
                    }
                }
            };

            _mockCourseRepository.Setup(repo => repo.GetCourseForAnalyticsAsync(courseId)).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseAnalyticsAsync(courseId);

            // Assert
            result.Should().NotBeNull();
            result.TotalStudentsEnrolled.Should().Be(3);
            result.StudentsCompleted.Should().Be(2);
            result.AverageScoreForCourse.Should().Be(80);
        }
    }
}