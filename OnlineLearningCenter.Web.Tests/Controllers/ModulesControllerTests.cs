using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.Web.Controllers;
using System.Threading.Tasks;
using Xunit;

namespace OnlineLearningCenter.Web.Tests.Controllers
{
    public class ModulesControllerTests
    {
        private readonly Mock<IModuleService> _mockModuleService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ModulesController _controller;

        public ModulesControllerTests()
        {
            _mockModuleService = new Mock<IModuleService>();
            _mockCourseService = new Mock<ICourseService>();
            _mockMapper = new Mock<IMapper>();

            _controller = new ModulesController(
                _mockModuleService.Object,
                _mockCourseService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Create_Get_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            var nonExistentCourseId = 99;
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(nonExistentCourseId)).ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.Create(nonExistentCourseId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Get_ShouldReturnViewWithCorrectCourseId_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var courseDto = new CourseDto { CourseId = courseId, Title = "Test Course" };
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(courseDto);

            // Act
            var result = await _controller.Create(courseId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CreateModuleDto>().Subject;
            model.CourseId.Should().Be(courseId);
            viewResult.ViewData["CourseTitle"].Should().Be("Test Course");
        }

        [Fact]
        public async Task Create_Post_ShouldCallServiceAndRedirectToCourseDetails_WhenModelIsValid()
        {
            // Arrange
            var createModuleDto = new CreateModuleDto { Title = "New Module", CourseId = 1 };

            // Act
            var result = await _controller.Create(createModuleDto);

            // Assert
            _mockModuleService.Verify(s => s.CreateModuleAsync(createModuleDto), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Details");
            redirectResult.ControllerName.Should().Be("Courses");
            redirectResult.RouteValues["id"].Should().Be(createModuleDto.CourseId);
        }
    }
}