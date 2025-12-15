using OnlineLearningCenter.DataAccess.Entities;
using System.Threading.Tasks; 

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<(List<Student> Items, int TotalCount)> GetPaginatedStudentsAsync(string? searchString, int pageNumber, int pageSize);
    Task<List<(Student Student, double AverageScore)>> GetStudentRankingsAsync(int? courseId);
    Task<IEnumerable<Student>> GetStudentsEnrolledInCourseAsync(int courseId);
    Task<List<Enrollment>> GetEnrollmentsWithDetailsAsync(int studentId);
}