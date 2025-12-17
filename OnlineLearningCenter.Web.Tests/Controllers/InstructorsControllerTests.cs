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
using System.Threading.Tasks;
using Xunit;

namespace OnlineLearningCenter.Web.Tests.Controllers
{
    public class InstructorsControllerTests
    {
        private readonly Mock<IInstructorService> _mockInstructorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly InstructorsController _controller;

        public InstructorsControllerTests()
        {
            _mockInstructorService = new Mock<IInstructorService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new InstructorsController(_mockInstructorService.Object, _mockMapper.Object);

            var mockSession = new Mock<ISession>();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithPaginatedInstructors()
        {
            var instructors = new List<InstructorDto>();
            var paginatedList = new PaginatedList<InstructorDto>(instructors, 0, 1, 10);
            _mockInstructorService.Setup(s => s.GetPaginatedInstructorsAsync(It.IsAny<string>(), It.IsAny<int>()))
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
            viewResult.Model.Should().BeOfType<PaginatedList<InstructorDto>>();
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenInstructorDoesNotExist()
        {
            // Arrange
            _mockInstructorService.Setup(s => s.GetInstructorByIdAsync(It.IsAny<int>())).ReturnsAsync((InstructorDto)null);

            // Act
            var result = await _controller.Details(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
            var createDto = new CreateInstructorDto { FullName = "Новый Преподаватель" };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            _mockInstructorService.Verify(s => s.CreateInstructorAsync(createDto), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>()
                  .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Edit_Post_ShouldReturnNotFound_WhenIdDoesNotMatchDto()
        {
            // Arrange
            var updateDto = new UpdateInstructorDto { InstructorId = 2, FullName = "Test" };
            var id = 1;

            // Act
            var result = await _controller.Edit(id, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}