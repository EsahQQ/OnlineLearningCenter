using OnlineLearningCenter.DataAccess.Entities;
using System.Threading.Tasks; 

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetStudentWithProgressDataAsync(int studentId);

    Task<IEnumerable<(Student Student, double AverageScore)>> GetStudentRankingsAsync(int? courseId = null);
}