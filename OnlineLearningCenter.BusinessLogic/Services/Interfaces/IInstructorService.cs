using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces;

public interface IInstructorService
{
    Task<PaginatedList<InstructorDto>> GetPaginatedInstructorsAsync(string? searchString, int pageNumber);
    Task<IEnumerable<InstructorDto>> GetAllInstructorsForSelectListAsync();
    Task<InstructorDto?> GetInstructorByIdAsync(int id);
    Task<InstructorDto> CreateInstructorAsync(CreateInstructorDto instructorDto);
    Task UpdateInstructorAsync(UpdateInstructorDto instructorDto);
    Task<List<CourseDto>> DeleteInstructorAsync(int id);
}