using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class EnrollmentsController : Controller
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseService _courseService;
    private readonly IStudentService _studentService;

    public EnrollmentsController(IEnrollmentService enrollmentService, ICourseService courseService, IStudentService studentService)
    {
        _enrollmentService = enrollmentService;
        _courseService = courseService;
        _studentService = studentService;
    }

    // GET: /Enrollments/Create?studentId=5
    public async Task<IActionResult> Create(int studentId)
    {
        var student = await _studentService.GetStudentByIdAsync(studentId);
        if (student == null) return NotFound();

        var courses = await _courseService.GetAllCoursesForSelectListAsync();

        ViewBag.Student = student;
        ViewBag.Courses = courses; 
        return View();
    }

    // POST: /Enrollments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int studentId, int courseId)
    {
        await _enrollmentService.EnrollStudentAsync(studentId, courseId);
        return RedirectToAction("Details", "Students", new { id = studentId });
    }

    // POST: /Enrollments/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int studentId, int courseId)
    {
        await _enrollmentService.UnenrollStudentAsync(studentId, courseId);
        return RedirectToAction("Details", "Students", new { id = studentId });
    }
}