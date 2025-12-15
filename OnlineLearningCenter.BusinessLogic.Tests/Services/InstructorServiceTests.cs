using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entities = OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.BusinessLogic.Tests.Services;

public class InstructorServiceTests
{
    private readonly Mock<IInstructorRepository> _mockInstructorRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly IMapper _mapper;
    private readonly InstructorService _instructorService;

    public InstructorServiceTests()
    {
        _mockInstructorRepository = new Mock<IInstructorRepository>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<BusinessLogic.Mappings.MappingProfile>(); });
        _mapper = mapperConfig.CreateMapper();
        _instructorService = new InstructorService(_mockInstructorRepository.Object, _mockCourseRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetPaginatedInstructorsAsync_ShouldReturnPaginatedData()
    {
        // Arrange
        var instructors = new List<Entities.Instructor> { new Entities.Instructor { InstructorId = 1, FullName = "Иван Петров" } };
        _mockInstructorRepository.Setup(r => r.GetPaginatedInstructorsAsync(null, 1, 10)).ReturnsAsync((instructors, 1));

        // Act
        var result = await _instructorService.GetPaginatedInstructorsAsync(null, 1);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.First().FullName.Should().Be("Иван Петров");
    }

    [Fact]
    public async Task GetInstructorByIdAsync_ShouldReturnInstructor_WhenExists()
    {
        // Arrange
        var instructorId = 1;
        var instructor = new Entities.Instructor { InstructorId = instructorId, FullName = "Иван Петров" };
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(instructorId)).ReturnsAsync(instructor);

        // Act
        var result = await _instructorService.GetInstructorByIdAsync(instructorId);

        // Assert
        result.Should().NotBeNull();
        result.InstructorId.Should().Be(instructorId);
    }

    [Fact]
    public async Task GetInstructorByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Entities.Instructor)null);

        // Act
        var result = await _instructorService.GetInstructorByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateInstructorAsync_ShouldCallRepositoryAdd()
    {
        // Arrange
        var createDto = new CreateInstructorDto { FullName = "Новый Преподаватель" };

        // Act
        await _instructorService.CreateInstructorAsync(createDto);

        // Assert
        _mockInstructorRepository.Verify(r => r.AddAsync(It.Is<Entities.Instructor>(i => i.FullName == createDto.FullName)), Times.Once);
    }

    [Fact]
    public async Task UpdateInstructorAsync_ShouldCallRepositoryUpdate_WhenExists()
    {
        // Arrange
        var instructorId = 1;
        var updateDto = new UpdateInstructorDto { InstructorId = instructorId, FullName = "Обновленное Имя" };
        var existingInstructor = new Entities.Instructor { InstructorId = instructorId, FullName = "Старое Имя" };
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(instructorId)).ReturnsAsync(existingInstructor);

        // Act
        await _instructorService.UpdateInstructorAsync(updateDto);

        // Assert
        _mockInstructorRepository.Verify(r => r.UpdateAsync(It.Is<Entities.Instructor>(i => i.FullName == updateDto.FullName)), Times.Once);
    }

    [Fact]
    public async Task UpdateInstructorAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateInstructorDto { InstructorId = 99 };
        _mockInstructorRepository.Setup(r => r.GetByIdAsync(updateDto.InstructorId)).ReturnsAsync((Entities.Instructor)null);

        // Act
        Func<Task> act = async () => await _instructorService.UpdateInstructorAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteInstructorAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        var instructorId = 1;

        // Act
        await _instructorService.DeleteInstructorAsync(instructorId);

        // Assert
        _mockInstructorRepository.Verify(r => r.DeleteAsync(instructorId), Times.Once);
    }
}