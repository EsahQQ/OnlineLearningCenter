using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
using OnlineLearningCenter.Web.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineLearningCenter.Web.Tests.Controllers
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICertificateService> _mockCertificateService;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _mockStudentService = new Mock<IStudentService>();
            _mockMapper = new Mock<IMapper>();
            _mockCertificateService = new Mock<ICertificateService>();

            _controller = new StudentsController(
                _mockStudentService.Object,
                _mockMapper.Object,
                _mockCertificateService.Object);

            var mockSession = new Mock<ISession>();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithPaginatedStudents()
        {
            var students = new List<StudentDto>();
            var paginatedList = new PaginatedList<StudentDto>(students, 0, 1, 10);
            _mockStudentService.Setup(s => s.GetPaginatedStudentsAsync(It.IsAny<string>(), It.IsAny<int>()))
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

            var result = await _controller.Index(null, 1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<PaginatedList<StudentDto>>();
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithProgress_WhenStudentExists()
        {
            // Arrange
            var studentId = 1;
            var studentDto = new StudentDto { StudentId = studentId, FullName = "Test Student" };
            var progressList = new List<StudentCourseProgressDto> { new StudentCourseProgressDto() };
            var certificates = new List<CertificateDto>();

            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync(studentDto);
            _mockStudentService.Setup(s => s.GetStudentProgressAsync(studentId)).ReturnsAsync(progressList);
            _mockCertificateService.Setup(s => s.GetCertificatesByStudentIdAsync(studentId)).ReturnsAsync(certificates);

            // Act
            var result = await _controller.Details(studentId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<StudentCourseProgressDto>>().Subject;
            model.Should().BeSameAs(progressList);

            viewResult.ViewData["Student"].Should().Be(studentDto);
            viewResult.ViewData["Certificates"].Should().Be(certificates);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            _mockStudentService.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync((StudentDto)null);

            // Act
            var result = await _controller.Details(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
            var createDto = new CreateStudentDto { FullName = "Test", Email = "test@test.com" };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            _mockStudentService.Verify(s => s.CreateStudentAsync(createDto), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>()
                  .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Details_ShouldSetViewBagCorrectly_WhenStudentExists()
        {
            // Arrange
            var studentId = 1;
            var studentDto = new StudentDto { StudentId = studentId, FullName = "Иван" };

            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync(studentDto);

            _mockStudentService.Setup(s => s.GetStudentProgressAsync(studentId)).ReturnsAsync(new List<StudentCourseProgressDto>());
            _mockCertificateService.Setup(s => s.GetCertificatesByStudentIdAsync(studentId)).ReturnsAsync(new List<CertificateDto>());

            // Act
            var result = await _controller.Details(studentId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["Student"].Should().Be(studentDto); 
        }
    }
}