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

public class ModuleServiceTests
{
    private readonly Mock<IModuleRepository> _mockModuleRepository;
    private readonly IMapper _mapper;
    private readonly ModuleService _moduleService;

    public ModuleServiceTests()
    {
        _mockModuleRepository = new Mock<IModuleRepository>();
        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<BusinessLogic.Mappings.MappingProfile>(); });
        _mapper = mapperConfig.CreateMapper();
        _moduleService = new ModuleService(_mockModuleRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetModulesByCourseIdAsync_ShouldReturnOnlyModulesForGivenCourse_AndBeSorted()
    {
        // Arrange
        var courseId = 1;
        var allModules = new List<Entities.Module>
        {
            new Entities.Module { ModuleId = 1, CourseId = courseId, Title = "Модуль 2", OrderNumber = 2 },
            new Entities.Module { ModuleId = 2, CourseId = 99, Title = "Чужой модуль" },
            new Entities.Module { ModuleId = 3, CourseId = courseId, Title = "Модуль 1", OrderNumber = 1 }
        };
        _mockModuleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(allModules);

        // Act
        var result = await _moduleService.GetModulesByCourseIdAsync(courseId);

        // Assert
        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Title.Should().Be("Модуль 1");
        result.All(m => m.CourseId == courseId).Should().BeTrue();
    }

    [Fact]
    public async Task GetModuleByIdAsync_ShouldReturnModule_WhenExists()
    {
        // Arrange
        var moduleId = 1;
        var module = new Entities.Module { ModuleId = moduleId, Title = "Тестовый модуль" };
        _mockModuleRepository.Setup(r => r.GetByIdAsync(moduleId)).ReturnsAsync(module);

        // Act
        var result = await _moduleService.GetModuleByIdAsync(moduleId);

        // Assert
        result.Should().NotBeNull();
        result.ModuleId.Should().Be(moduleId);
    }

    [Fact]
    public async Task CreateModuleAsync_ShouldCallRepositoryAdd()
    {
        // Arrange
        var createDto = new CreateModuleDto { Title = "Новый модуль", CourseId = 1 };

        // Act
        await _moduleService.CreateModuleAsync(createDto);

        // Assert
        _mockModuleRepository.Verify(r => r.AddAsync(It.Is<Entities.Module>(m => m.Title == createDto.Title)), Times.Once);
    }

    [Fact]
    public async Task UpdateModuleAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateModuleDto { ModuleId = 99 };
        _mockModuleRepository.Setup(r => r.GetByIdAsync(updateDto.ModuleId)).ReturnsAsync((Entities.Module)null);

        // Act
        Func<Task> act = async () => await _moduleService.UpdateModuleAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteModuleAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        var moduleId = 1;

        // Act
        await _moduleService.DeleteModuleAsync(moduleId);

        // Assert
        _mockModuleRepository.Verify(r => r.DeleteAsync(moduleId), Times.Once);
    }
}