using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
    public class CoursesController : Controller
    {
        // Внедряем сервисы и AutoMapper вместо DbContext
        private readonly ICourseService _courseService;
        private readonly IInstructorService _instructorService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IInstructorService instructorService, IMapper mapper)
        {
            _courseService = courseService;
            _instructorService = instructorService;
            _mapper = mapper;
        }

        // GET: Courses - добавляем параметры для фильтрации
        public async Task<IActionResult> Index(string? category, string? difficulty, int? instructorId)
        {
            var courses = await _courseService.GetActiveCoursesAsync(category, difficulty, instructorId);

            // ViewBag.Instructors = ...

            return View(courses); 
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

            return View(course); 
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            // вспомогательный метод для загрузки преподавателей
            await PopulateInstructorsDropDownList();
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Вместо Entity Course принимаем CreateCourseDto
        public async Task<IActionResult> Create(CreateCourseDto courseDto)
        {
            // 3. ModelState.IsValid проверяет атрибуты валидации из CreateCourseDto
            if (ModelState.IsValid)
            {
                await _courseService.CreateCourseAsync(courseDto);
                return RedirectToAction(nameof(Index));
            }

            // Если модель невалидна, снова загружаем преподавателей и возвращаем View с теми же данными
            await PopulateInstructorsDropDownList(courseDto.InstructorId);
            return View(courseDto);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            // нужно получить InstructorId для выпадающего списка.

            // Для формы редактирования нужен объект с полями, которые пользователь будет менять.
            var courseToEdit = _mapper.Map<CreateCourseDto>(course);

            await PopulateInstructorsDropDownList(course.InstructorId); // ID текущего преподавателя
            return View(courseToEdit);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateCourseDto courseDto)
        {
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

        // Вспомогательный метод
        private async Task PopulateInstructorsDropDownList(object? selectedInstructor = null)
        {
            var instructors = await _instructorService.GetAllInstructorsAsync();
            ViewBag.InstructorId = new SelectList(instructors, "InstructorId", "FullName", selectedInstructor);
        }
    }
}