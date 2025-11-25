using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
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
        private readonly IMapper _mapper;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BusinessLogic.Mappings.MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _courseService = new CourseService(_mockCourseRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetPaginatedCoursesAsync_ShouldReturnPaginatedListOfCourseDtos()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10; 
            var searchString = "Test";

            var coursesFromRepo = new List<Entities.Course> { new Entities.Course { CourseId = 1, Title = "Test Course", Instructor = new Entities.Instructor() } };

            _mockCourseRepository
                .Setup(r => r.GetPaginatedCoursesAsync(searchString, null, null, null, true, pageNumber, pageSize))
                .ReturnsAsync((coursesFromRepo, 1));

            // Act
            var result = await _courseService.GetPaginatedCoursesAsync(searchString, null, null, null, true, pageNumber);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PaginatedList<CourseDto>>();
            result.TotalCount.Should().Be(1);
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Test Course");
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourseDto_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var course = new Entities.Course { CourseId = courseId, Title = "Test Course", Instructor = new Entities.Instructor { FullName = "Test Instructor" } };

            _mockCourseRepository.Setup(repo => repo.GetCourseWithDetailsAsync(courseId)).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CourseDto>();
            result.CourseId.Should().Be(courseId);
            result.Title.Should().Be("Test Course");
            result.InstructorName.Should().Be("Test Instructor");
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

            // Act
            await _courseService.CreateCourseAsync(createCourseDto);

            // Assert
            _mockCourseRepository.Verify(repo => repo.AddAsync(It.Is<Entities.Course>(c => c.Title == createCourseDto.Title)), Times.Once);
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

        [Fact]
        public async Task UpdateCourseAsync_ShouldCallGetAndUpdate_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var updateDto = new UpdateCourseDto { CourseId = courseId, Title = "Updated Title" };
            var existingCourse = new Entities.Course { CourseId = courseId, Title = "Old Title" };

            _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(existingCourse);

            // Act
            await _courseService.UpdateCourseAsync(courseId, updateDto);

            // Assert
            _mockCourseRepository.Verify(r => r.GetByIdAsync(courseId), Times.Once);
            _mockCourseRepository.Verify(r => r.UpdateAsync(It.Is<Entities.Course>(c => c.Title == "Updated Title")), Times.Once);
        }
    }
}