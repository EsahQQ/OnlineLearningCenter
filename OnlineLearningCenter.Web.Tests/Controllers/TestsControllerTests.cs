using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OnlineLearningCenter.Web.Tests.Controllers
{
    public class TestsControllerTests
    {
        private readonly Mock<ITestService> _mockTestService;
        private readonly Mock<IModuleService> _mockModuleService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ITestResultService> _mockResultService;
        private readonly TestsController _controller;

        public TestsControllerTests()
        {
            _mockTestService = new Mock<ITestService>();
            _mockModuleService = new Mock<IModuleService>();
            _mockMapper = new Mock<IMapper>();
            _mockResultService = new Mock<ITestResultService>();

            _controller = new TestsController(
                _mockTestService.Object,
                _mockModuleService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithTestsForCorrectModule()
        {
            // Arrange
            var moduleId = 1;
            var moduleDto = new ModuleDto { ModuleId = moduleId, Title = "Test Module", CourseId = 10 };
            var tests = new List<TestDto> { new TestDto { Title = "Test 1" } };

            _mockModuleService.Setup(s => s.GetModuleByIdAsync(moduleId)).ReturnsAsync(moduleDto);
            _mockTestService.Setup(s => s.GetTestsByModuleIdAsync(moduleId)).ReturnsAsync(tests);

            // Act
            var result = await _controller.Index(moduleId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["ModuleTitle"].Should().Be("Test Module");
            viewResult.ViewData["CourseId"].Should().Be(10);

            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<TestDto>>().Subject;
            model.Should().ContainSingle();
        }

        [Fact]
        public async Task Create_Post_ShouldCallServiceAndRedirectToIndex()
        {
            // Arrange
            var createTestDto = new CreateTestDto { Title = "New Test", ModuleId = 1 };

            // Act
            var result = await _controller.Create(createTestDto);

            // Assert
            _mockTestService.Verify(s => s.CreateTestAsync(createTestDto), Times.Once);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.RouteValues["moduleId"].Should().Be(createTestDto.ModuleId);
        }
    }
}