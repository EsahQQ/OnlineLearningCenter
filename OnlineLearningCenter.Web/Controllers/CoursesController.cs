using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.Web.ViewModels;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IInstructorService _instructorService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IInstructorService instructorService, IMapper mapper)
        {
            _courseService = courseService;
            _instructorService = instructorService;
            _mapper = mapper;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string? category, string? difficulty, int? instructorId, bool showOnlyActive = false, int pageNumber = 1)
        {
            var paginatedCourses = await _courseService.GetPaginatedCoursesAsync(
                category, difficulty, instructorId, showOnlyActive, pageNumber);

            var instructors = await _instructorService.GetAllInstructorsAsync();
            var categories = await _courseService.GetAllCategoriesAsync();
            var difficulties = await _courseService.GetAllDifficultiesAsync();

            ViewBag.Instructors = new SelectList(instructors, "InstructorId", "FullName", instructorId);
            ViewBag.Categories = new SelectList(categories, category);
            ViewBag.Difficulties = new SelectList(difficulties, difficulty);

            ViewBag.ShowOnlyActive = showOnlyActive;

            ViewData["CurrentCategory"] = category;
            ViewData["CurrentDifficulty"] = difficulty;
            ViewData["CurrentInstructorId"] = instructorId;
            ViewData["CurrentShowOnlyActive"] = showOnlyActive;

            return View(paginatedCourses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            var analytics = await _courseService.GetCourseAnalyticsAsync(id.Value);

            var viewModel = new CourseDetailsViewModel
            {
                Course = course,
                Analytics = analytics
            };

            return View(viewModel);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            await PopulateInstructorsDropDownList();
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseDto courseDto)
        {
            if (ModelState.IsValid)
            {
                await _courseService.CreateCourseAsync(courseDto);
                return RedirectToAction(nameof(Index));
            }

            await PopulateInstructorsDropDownList(courseDto.InstructorId);
            return View(courseDto);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var courseDto = await _courseService.GetCourseByIdAsync(id.Value);
            if (courseDto == null) return NotFound();

            var courseToEdit = _mapper.Map<UpdateCourseDto>(courseDto);
            await PopulateInstructorsDropDownList(courseDto.InstructorId);
            return View(courseToEdit);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCourseDto courseDto)
        {
            if (id != courseDto.CourseId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateCourseAsync(id, courseDto);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateInstructorsDropDownList(courseDto.InstructorId);
            return View(courseDto);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateInstructorsDropDownList(object? selectedInstructor = null)
        {
            var instructors = await _instructorService.GetAllInstructorsAsync();
            ViewBag.InstructorId = new SelectList(instructors, "InstructorId", "FullName", selectedInstructor);
        }
    }
}