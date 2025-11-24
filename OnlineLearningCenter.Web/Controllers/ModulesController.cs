using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

public class ModulesController : Controller
{
    private readonly IModuleService _moduleService;
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public ModulesController(IModuleService moduleService, ICourseService courseService, IMapper mapper)
    {
        _moduleService = moduleService;
        _courseService = courseService;
        _mapper = mapper;
    }

    // GET: Modules/Create?courseId=5
    public async Task<IActionResult> Create(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null) return NotFound();

        ViewBag.CourseTitle = course.Title;
        ViewBag.CourseId = course.CourseId;

        var model = new CreateModuleDto { CourseId = courseId };
        return View(model);
    }

    // POST: Modules/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateModuleDto moduleDto)
    {
        if (ModelState.IsValid)
        {
            await _moduleService.CreateModuleAsync(moduleDto);
            return RedirectToAction("Details", "Courses", new { id = moduleDto.CourseId });
        }

        var course = await _courseService.GetCourseByIdAsync(moduleDto.CourseId);
        ViewBag.CourseTitle = course?.Title;
        ViewBag.CourseId = course?.CourseId;
        return View(moduleDto);
    }

    // GET: Modules/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var module = await _moduleService.GetModuleByIdAsync(id);
        if (module == null) return NotFound();

        var course = await _courseService.GetCourseByIdAsync(module.CourseId);
        ViewBag.CourseTitle = course?.Title;
        ViewBag.CourseId = course?.CourseId;

        var model = _mapper.Map<UpdateModuleDto>(module);
        return View(model);
    }

    // POST: Modules/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateModuleDto moduleDto)
    {
        if (id != moduleDto.ModuleId) return NotFound();

        if (ModelState.IsValid)
        {
            await _moduleService.UpdateModuleAsync(moduleDto);
            return RedirectToAction("Details", "Courses", new { id = moduleDto.CourseId });
        }

        var course = await _courseService.GetCourseByIdAsync(moduleDto.CourseId);
        ViewBag.CourseTitle = course?.Title;
        ViewBag.CourseId = course?.CourseId;
        return View(moduleDto);
    }

    // GET: Modules/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var module = await _moduleService.GetModuleByIdAsync(id.Value);
        if (module == null) return NotFound();

        var course = await _courseService.GetCourseByIdAsync(module.CourseId);
        ViewBag.CourseTitle = course?.Title;


        return View(module);
    }

    // POST: Modules/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int courseId) 
    {
        await _moduleService.DeleteModuleAsync(id);
        return RedirectToAction("Details", "Courses", new { id = courseId });
    }
}