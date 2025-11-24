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

        public async Task<IActionResult> Index(int? courseId, int pageNumber = 1)
        {
            var rankings = await _studentService.GetStudentRankingsAsync(courseId, pageNumber);

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
}