using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using OnlineLearningCenter.BusinessLogic.Services.Interfaces;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class InstructorsController : Controller
{
    private readonly IInstructorService _instructorService;
    private readonly IMapper _mapper;

    public InstructorsController(IInstructorService instructorService, IMapper mapper)
    {
        _instructorService = instructorService;
        _mapper = mapper;
    }

    // GET: Instructors
    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1, string? clearFilter = null)
    {
        if (clearFilter != null)
        {
            HttpContext.Session.Remove("Instructors_Search");
            return RedirectToAction(nameof(Index));
        }

        if (HttpContext.Request.Query.ContainsKey(nameof(searchString)))
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                HttpContext.Session.SetString("Instructors_Search", searchString);
            }
            else
            {
                HttpContext.Session.Remove("Instructors_Search");
            }
        }
        else
        {
            searchString = HttpContext.Session.GetString("Instructors_Search");
        }

        var instructors = await _instructorService.GetPaginatedInstructorsAsync(searchString, pageNumber);

        ViewData["CurrentFilter"] = searchString;

        return View(instructors);
    }

    // GET: Instructors/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var instructor = await _instructorService.GetInstructorByIdAsync(id.Value);
        if (instructor == null) return NotFound();
        return View(instructor);
    }

    // GET: Instructors/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Instructors/Create
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInstructorDto instructorDto)
    {
        if (ModelState.IsValid)
        {
            await _instructorService.CreateInstructorAsync(instructorDto);
            return RedirectToAction(nameof(Index));
        }
        return View(instructorDto);
    }

    // GET: Instructors/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var instructor = await _instructorService.GetInstructorByIdAsync(id.Value);
        if (instructor == null) return NotFound();

        var instructorToUpdate = _mapper.Map<UpdateInstructorDto>(instructor);
        return View(instructorToUpdate);
    }

    // POST: Instructors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, UpdateInstructorDto instructorDto)
    {
        if (id != instructorDto.InstructorId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _instructorService.UpdateInstructorAsync(instructorDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(instructorDto);
    }

    // GET: Instructors/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        if (TempData["BlockingCourses"] is string blockingCoursesJson)
        {
            var blockingCourses = System.Text.Json.JsonSerializer.Deserialize<List<CourseDto>>(blockingCoursesJson);
            ViewBag.BlockingCourses = blockingCourses;
        }

        var instructor = await _instructorService.GetInstructorByIdAsync(id.Value);
        if (instructor == null) return NotFound();
        return View(instructor);
    }

    // POST: Instructors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var blockingCourses = await _instructorService.DeleteInstructorAsync(id);

        if (blockingCourses.Any())
        {
            TempData["ErrorMessage"] = "Невозможно удалить преподавателя, так как за ним закреплены следующие курсы:";
            TempData["BlockingCourses"] = System.Text.Json.JsonSerializer.Serialize(blockingCourses);

            return RedirectToAction(nameof(Delete), new { id = id });
        }

        TempData["SuccessMessage"] = "Преподаватель успешно удален.";
        return RedirectToAction(nameof(Index));
    }
}