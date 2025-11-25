using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.Web.Controllers;
using System.Collections.Generic;
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
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithPaginatedStudents()
        {
            // Arrange
            var students = new List<StudentDto>();
            var paginatedList = new PaginatedList<StudentDto>(students, 0, 1, 10);
            _mockStudentService.Setup(s => s.GetPaginatedStudentsAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.Index(null, 1);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<PaginatedList<StudentDto>>();
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
    }
}