using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace OnlineLearningCenter.BusinessLogic.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    public EnrollmentService(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task EnrollStudentAsync(int studentId, int courseId)
    {
        var allEnrollments = await _enrollmentRepository.GetAllAsync();
        if (allEnrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        {
            return;
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            Progress = 0
        };
        await _enrollmentRepository.AddAsync(enrollment);
    }

    public async Task UnenrollStudentAsync(int studentId, int courseId)
    {
        var allEnrollments = await _enrollmentRepository.GetAllAsync();
        var enrollmentToDelete = allEnrollments.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

        if (enrollmentToDelete != null)
        {
            await _enrollmentRepository.DeleteAsync(enrollmentToDelete.EnrollmentId);
        }
    }
}