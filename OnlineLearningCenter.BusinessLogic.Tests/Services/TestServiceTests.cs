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

namespace OnlineLearningCenter.BusinessLogic.Tests.Services;

public class TestServiceTests
{
    private readonly Mock<ITestRepository> _mockTestRepository;
    private readonly IMapper _mapper;
    private readonly TestService _testService;

    public TestServiceTests()
    {
        _mockTestRepository = new Mock<ITestRepository>();
        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<BusinessLogic.Mappings.MappingProfile>(); });
        _mapper = mapperConfig.CreateMapper();
        _testService = new TestService(_mockTestRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetTestsByModuleIdAsync_ShouldReturnOnlyTestsForGivenModule()
    {
        // Arrange
        var moduleId = 1;
        var allTests = new List<Entities.Test>
        {
            new Entities.Test { TestId = 1, ModuleId = moduleId, Title = "Тест А" },
            new Entities.Test { TestId = 2, ModuleId = 99, Title = "Чужой тест" },
            new Entities.Test { TestId = 3, ModuleId = moduleId, Title = "Тест Б" }
        };
        _mockTestRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(allTests);

        // Act
        var result = await _testService.GetTestsByModuleIdAsync(moduleId);

        // Assert
        result.Should().NotBeNull().And.HaveCount(2);
        result.All(t => t.ModuleId == moduleId).Should().BeTrue();
    }

    [Fact]
    public async Task GetTestByIdAsync_ShouldReturnTest_WhenExists()
    {
        // Arrange
        var testId = 1;
        var test = new Entities.Test { TestId = testId, Title = "Тестовый тест" };
        _mockTestRepository.Setup(r => r.GetByIdAsync(testId)).ReturnsAsync(test);

        // Act
        var result = await _testService.GetTestByIdAsync(testId);

        // Assert
        result.Should().NotBeNull();
        result.TestId.Should().Be(testId);
    }

    [Fact]
    public async Task CreateTestAsync_ShouldCallRepositoryAdd()
    {
        // Arrange
        var createDto = new CreateTestDto { Title = "Новый тест", ModuleId = 1 };

        _mockTestRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Entities.Test());

        // Act
        await _testService.CreateTestAsync(createDto);

        // Assert
        _mockTestRepository.Verify(r => r.AddAsync(It.Is<Entities.Test>(t => t.Title == createDto.Title)), Times.Once);
    }

    [Fact]
    public async Task UpdateTestAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateTestDto { TestId = 99 };
        _mockTestRepository.Setup(r => r.GetByIdAsync(updateDto.TestId)).ReturnsAsync((Entities.Test)null);

        // Act
        Func<Task> act = async () => await _testService.UpdateTestAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteTestAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        var testId = 1;

        // Act
        await _testService.DeleteTestAsync(testId);

        // Assert
        _mockTestRepository.Verify(r => r.DeleteAsync(testId), Times.Once);
    }
}