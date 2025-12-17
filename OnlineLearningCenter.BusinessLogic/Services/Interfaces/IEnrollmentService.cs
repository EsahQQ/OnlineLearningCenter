using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces;

public interface IEnrollmentService
{
    Task EnrollStudentAsync(int studentId, int courseId);
    Task UnenrollStudentAsync(int studentId, int courseId);
}