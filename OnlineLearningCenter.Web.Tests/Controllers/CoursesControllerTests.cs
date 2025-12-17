using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.Web.Controllers;
using OnlineLearningCenter.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;

namespace OnlineLearningCenter.Web.Tests.Controllers
{
    public class CoursesControllerTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<IInstructorService> _mockInstructorService;
        private readonly Mock<IModuleService> _mockModuleService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _mockInstructorService = new Mock<IInstructorService>();
            _mockModuleService = new Mock<IModuleService>();
            _mockMapper = new Mock<IMapper>();

            _controller = new CoursesController(
                _mockCourseService.Object,
                _mockInstructorService.Object,
                _mockMapper.Object,
                _mockModuleService.Object);

            var mockSession = new Mock<ISession>();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewResult_WithPaginatedListOfCourses()
        {
            var courseDtos = new List<CourseDto> { new CourseDto(), new CourseDto() };
            var paginatedList = new PaginatedList<CourseDto>(courseDtos, 2, 1, 10);

            _mockCourseService.Setup(s => s.GetPaginatedCoursesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<int>()))
                .ReturnsAsync(paginatedList);

            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var sessionMock = new Mock<ISession>();

            requestMock.Setup(r => r.Query).Returns(new QueryCollection());
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var result = await _controller.Index(null, null, null, null, true, 1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<PaginatedList<CourseDto>>().Subject;
            model.Should().HaveCount(2);
        }

        [Fact]
        public async Task Details_ShouldReturnViewResult_WithViewModel_WhenCourseExists()
        {
            // Arrange
            var courseId = 1;
            var courseDto = new CourseDto { CourseId = courseId, Title = "Test" };
            var analyticsDto = new CourseAnalyticsDto();
            var modules = new List<ModuleDto>();

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(courseDto);
            _mockCourseService.Setup(s => s.GetCourseAnalyticsAsync(courseId)).ReturnsAsync(analyticsDto);
            _mockModuleService.Setup(s => s.GetModulesByCourseIdAsync(courseId)).ReturnsAsync(modules);

            // Act
            var result = await _controller.Details(courseId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CourseDetailsViewModel>().Subject;
            model.Course.Should().BeEquivalentTo(courseDto);
            model.Analytics.Should().Be(analyticsDto);
            model.Modules.Should().BeSameAs(modules);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = 99;
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.Details(courseId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Post_ShouldCallServiceAndRedirect_WhenModelStateIsValid()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto { Title = "New Course" };

            // Act
            var result = await _controller.Create(createCourseDto);

            // Assert
            _mockCourseService.Verify(s => s.CreateCourseAsync(createCourseDto), Times.Once);
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.Create(createCourseDto);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(createCourseDto);
            _mockCourseService.Verify(s => s.CreateCourseAsync(It.IsAny<CreateCourseDto>()), Times.Never);
        }
    }
}