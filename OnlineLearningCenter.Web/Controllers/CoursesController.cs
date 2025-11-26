using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.Web.ViewModels;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IInstructorService _instructorService;
    private readonly IModuleService _moduleService;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService courseService, IInstructorService instructorService, IMapper mapper, IModuleService moduleService)
    {
        _courseService = courseService;
        _instructorService = instructorService;
        _mapper = mapper;
        _moduleService = moduleService;
    }

    // GET: Courses
    public async Task<IActionResult> Index(
        string? searchString,
        string? category,
        string? difficulty,
        int? instructorId,
        bool? showOnlyActive,
        int pageNumber = 1,
        string? clearFilter = null)
    {

        if (clearFilter != null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        if (searchString != null) HttpContext.Session.SetString("Courses_Search", searchString);
        if (category != null) HttpContext.Session.SetString("Courses_Category", category);
        if (difficulty != null) HttpContext.Session.SetString("Courses_Difficulty", difficulty);
        if (instructorId != null) HttpContext.Session.SetInt32("Courses_InstructorId", instructorId.Value);
        if (showOnlyActive != null) HttpContext.Session.SetString("Courses_ShowActive", showOnlyActive.Value.ToString());

        var finalSearchString = searchString ?? HttpContext.Session.GetString("Courses_Search");
        var finalCategory = category ?? HttpContext.Session.GetString("Courses_Category");
        var finalDifficulty = difficulty ?? HttpContext.Session.GetString("Courses_Difficulty");
        var finalInstructorId = instructorId ?? HttpContext.Session.GetInt32("Courses_InstructorId");

        var showActiveString = HttpContext.Session.GetString("Courses_ShowActive");
        var finalShowOnlyActive = showOnlyActive ?? (showActiveString != null ? bool.Parse(showActiveString) : true); // По умолчанию true

        var paginatedCourses = await _courseService.GetPaginatedCoursesAsync(finalSearchString,
            finalCategory, finalDifficulty, finalInstructorId, finalShowOnlyActive, pageNumber);

        var instructors = await _instructorService.GetAllInstructorsForSelectListAsync();
        var categories = await _courseService.GetAllCategoriesAsync();
        var difficulties = await _courseService.GetAllDifficultiesAsync();

        ViewBag.Instructors = new SelectList(instructors, "InstructorId", "FullName", finalInstructorId);
        ViewBag.Categories = new SelectList(categories, finalCategory);
        ViewBag.Difficulties = new SelectList(difficulties, finalDifficulty);
        ViewBag.ShowOnlyActive = finalShowOnlyActive;

        ViewData["CurrentFilter"] = finalSearchString;
        ViewData["CurrentCategory"] = finalCategory;
        ViewData["CurrentDifficulty"] = finalDifficulty;
        ViewData["CurrentInstructorId"] = finalInstructorId;
        ViewData["CurrentShowOnlyActive"] = finalShowOnlyActive;

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

        var modules = await _moduleService.GetModulesByCourseIdAsync(id.Value);

        var viewModel = new CourseDetailsViewModel
        {
            Course = course,
            Analytics = analytics,
            Modules = modules
        };

        return View(viewModel);
    }

    // GET: Courses/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await PopulateInstructorsDropDownList();
        return View();
    }

    // POST: Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _courseService.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateInstructorsDropDownList(object? selectedInstructor = null)
    {
        var instructors = await _instructorService.GetAllInstructorsForSelectListAsync();
        ViewBag.InstructorId = new SelectList(instructors, "InstructorId", "FullName", selectedInstructor);
    }
}