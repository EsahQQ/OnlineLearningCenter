using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentCourseProgressDto>> GetStudentProgressAsync(int studentId);
        Task<PaginatedList<StudentDto>> GetPaginatedStudentsAsync(string? searchString, int pageNumber);
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task UpdateStudentAsync(UpdateStudentDto studentDto);
        Task DeleteStudentAsync(int id);
        Task<PaginatedList<StudentRankingDto>> GetStudentRankingsAsync(int? courseId, int pageNumber);
        Task<IEnumerable<StudentDto>> GetStudentsAvailableForTestAsync(int testId);
    }
}