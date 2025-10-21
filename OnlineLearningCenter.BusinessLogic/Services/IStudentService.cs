using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services
{
    public interface IStudentService
    {
        // Метод для нашего дополнительного требования
        Task<IEnumerable<StudentCourseProgressDto>> GetStudentProgressAsync(int studentId);

        // Методы для CRUD операций
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto);
        Task UpdateStudentAsync(UpdateStudentDto studentDto);
        Task DeleteStudentAsync(int id);
    }
}