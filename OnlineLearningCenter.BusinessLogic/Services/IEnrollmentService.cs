using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public interface IEnrollmentService
{
    Task EnrollStudentAsync(int studentId, int courseId);
    Task UnenrollStudentAsync(int studentId, int courseId);
}