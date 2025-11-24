using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

public class TestsController : Controller
{
    private readonly ITestService _testService;
    private readonly IModuleService _moduleService;
    private readonly IMapper _mapper;
    private readonly ITestResultService _resultService;

    public TestsController(ITestService testService, IModuleService moduleService, IMapper mapper, ITestResultService resultService)
    {
        _testService = testService;
        _moduleService = moduleService;
        _mapper = mapper;
        _resultService = resultService;
        _resultService = resultService;
    }

    // GET: /Tests?moduleId=5
    public async Task<IActionResult> Index(int moduleId)
    {
        var module = await _moduleService.GetModuleByIdAsync(moduleId);
        if (module == null) return NotFound("Модуль не найден.");

        ViewBag.ModuleTitle = module.Title;
        ViewBag.ModuleId = module.ModuleId;
        ViewBag.CourseId = module.CourseId; 

        var tests = await _testService.GetTestsByModuleIdAsync(moduleId);
        return View(tests);
    }

    // GET: Tests/Create?moduleId=5
    public async Task<IActionResult> Create(int moduleId)
    {
        var module = await _moduleService.GetModuleByIdAsync(moduleId);
        if (module == null) return NotFound("Модуль не найден.");

        ViewBag.ModuleTitle = module.Title;
        ViewBag.ModuleId = module.ModuleId;

        var model = new CreateTestDto { ModuleId = moduleId };
        return View(model);
    }

    // POST: Tests/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTestDto testDto)
    {
        if (ModelState.IsValid)
        {
            await _testService.CreateTestAsync(testDto);
            return RedirectToAction("Index", new { moduleId = testDto.ModuleId });
        }

        var module = await _moduleService.GetModuleByIdAsync(testDto.ModuleId);
        ViewBag.ModuleTitle = module?.Title;
        ViewBag.ModuleId = module?.ModuleId;
        return View(testDto);
    }

    // GET: Tests/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var test = await _testService.GetTestByIdAsync(id.Value);
        if (test == null) return NotFound();

        var module = await _moduleService.GetModuleByIdAsync(test.ModuleId);
        ViewBag.ModuleTitle = module?.Title;
        ViewBag.ModuleId = module?.ModuleId;

        var model = _mapper.Map<UpdateTestDto>(test);
        return View(model);
    }

    // POST: Tests/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateTestDto testDto)
    {
        if (id != testDto.TestId) return NotFound();

        if (ModelState.IsValid)
        {
            await _testService.UpdateTestAsync(testDto);
            return RedirectToAction("Index", new { moduleId = testDto.ModuleId });
        }

        var module = await _moduleService.GetModuleByIdAsync(testDto.ModuleId);
        ViewBag.ModuleTitle = module?.Title;
        ViewBag.ModuleId = module?.ModuleId;
        return View(testDto);
    }

    // GET: Tests/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var test = await _testService.GetTestByIdAsync(id.Value);
        if (test == null) return NotFound();

        var module = await _moduleService.GetModuleByIdAsync(test.ModuleId);
        ViewBag.ModuleTitle = module?.Title;

        return View(test);
    }

    // POST: Tests/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int testId, int moduleId)
    {
        await _testService.DeleteTestAsync(testId);
        return RedirectToAction("Index", new { moduleId = moduleId });
    }

    // GET: Tests/Results?testId=10
    public async Task<IActionResult> Results(int testId)
    {
        var test = await _testService.GetTestByIdAsync(testId);
        if (test == null) return NotFound("Тест не найден");

        ViewBag.TestTitle = test.Title;
        ViewBag.ModuleId = test.ModuleId;

        var results = await _resultService.GetResultsByTestIdAsync(testId);
        return View(results);
    }
}