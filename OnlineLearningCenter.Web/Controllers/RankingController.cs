using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;
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

        if (HttpContext.Request.Query.ContainsKey(nameof(courseId)))
        {
            if (courseId.HasValue)
            {
                HttpContext.Session.SetInt32("Ranking_CourseId", (int)courseId.Value);
            }
            else
            {
                HttpContext.Session.Remove("Ranking_CourseId");
            }
        }
        else
        {
            courseId = HttpContext.Session.GetInt32("Ranking_CourseId");
        }

        var rankings = await _studentService.GetStudentRankingsAsync((int?)courseId, pageNumber);

        var allCourses = await _courseService.GetAllCoursesForSelectListAsync();
        ViewBag.Courses = new SelectList(allCourses, "CourseId", "Title", courseId);

        ViewData["CurrentCourseId"] = courseId;

        if (courseId.HasValue)
        {
            var selectedCourse = allCourses.FirstOrDefault(c => c.CourseId == courseId.Value);
            ViewBag.SelectedCourseTitle = selectedCourse?.Title;
        }

        return View(rankings);
    }
}