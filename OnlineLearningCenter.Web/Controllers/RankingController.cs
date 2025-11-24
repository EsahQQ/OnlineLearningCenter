using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
    public class RankingController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public RankingController(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> Index(int? courseId)
        {
            var rankings = await _studentService.GetStudentRankingsAsync(courseId);

            var paginatedResult = await _courseService.GetPaginatedCoursesAsync(null, null, null, false, 1);
            var allCourses = paginatedResult.ToList();
            ViewBag.Courses = new SelectList(allCourses, "CourseId", "Title", courseId);

            ViewBag.SelectedCourseId = courseId;
            if (courseId.HasValue)
            {
                var selectedCourse = await _courseService.GetCourseByIdAsync(courseId.Value);
                ViewBag.SelectedCourseTitle = selectedCourse?.Title;
            }

            return View(rankings);
        }
    }
}