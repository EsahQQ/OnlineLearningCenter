using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class RankingController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;

    public RankingController(IStudentService studentService, ICourseService courseService)
    {
        _studentService = studentService;
        _courseService = courseService;
    }

    public async Task<IActionResult> Index(int? courseId, int pageNumber = 1, string? clearFilter = null)
    {
        if (clearFilter != null)
        {
            HttpContext.Session.Remove("Ranking_CourseId");
            return RedirectToAction(nameof(Index));
        }

        if (courseId != null)
        {
            HttpContext.Session.SetInt32("Ranking_CourseId", courseId.Value);
        }

        var finalCourseId = courseId ?? HttpContext.Session.GetInt32("Ranking_CourseId");

        var rankings = await _studentService.GetStudentRankingsAsync(finalCourseId, pageNumber);

        var allCourses = await _courseService.GetAllCoursesForSelectListAsync();
        ViewBag.Courses = new SelectList(allCourses, "CourseId", "Title", finalCourseId);

        ViewData["CurrentCourseId"] = finalCourseId;

        if (finalCourseId.HasValue)
        {
            var selectedCourse = allCourses.FirstOrDefault(c => c.CourseId == finalCourseId.Value);
            ViewBag.SelectedCourseTitle = selectedCourse?.Title;
        }

        return View(rankings);
    }
}