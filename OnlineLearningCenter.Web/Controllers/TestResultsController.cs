using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.DataAccess.Entities;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
    public class TestResultsController : Controller
    {
        private readonly ITestResultService _resultService;
        private readonly ITestService _testService;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public TestResultsController(ITestResultService resultService, ITestService testService, IStudentService studentService, IMapper mapper)
        {
            _resultService = resultService;
            _testService = testService;
            _studentService = studentService;
            _mapper = mapper;
        }

        // GET: /TestResults?testId=10
        public async Task<IActionResult> Index(int testId, int pageNumber = 1)
        {
            var test = await _testService.GetTestByIdAsync(testId);
            if (test == null) return NotFound("Тест не найден");

            ViewBag.TestTitle = test.Title;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.TestId = test.TestId;

            var results = await _resultService.GetPaginatedResultsByTestIdAsync(testId, pageNumber);

            ViewData["CurrentTestId"] = testId;

            return View(results);
        }

        // GET: TestResults/Create?testId=10
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int testId)
        {
            var test = await _testService.GetTestByIdAsync(testId);
            if (test == null) return NotFound();

            ViewBag.TestTitle = test.Title;

            var availableStudents = await _studentService.GetStudentsAvailableForTestAsync(testId);
            ViewBag.Students = new SelectList(availableStudents, "StudentId", "FullName");

            var model = new CreateTestResultDto { TestId = testId };
            return View(model);
        }

        // POST: TestResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTestResultDto resultDto)
        {
            if (ModelState.IsValid)
            {
                await _resultService.CreateResultAsync(resultDto);
                return RedirectToAction("Index", new { testId = resultDto.TestId });
            }

            var test = await _testService.GetTestByIdAsync(resultDto.TestId);
            ViewBag.TestTitle = test?.Title;
            var availableStudents = await _studentService.GetStudentsAvailableForTestAsync(resultDto.TestId);
            ViewBag.Students = new SelectList(availableStudents, "StudentId", "FullName");
            return View(resultDto);
        }

        // GET: TestResults/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var result = await _resultService.GetResultByIdAsync(id.Value);
            if (result == null) return NotFound();

            ViewBag.StudentFullName = result.StudentFullName;
            ViewBag.TestTitle = result.TestTitle;
            ViewBag.TestId = result.TestId;

            var model = _mapper.Map<UpdateTestResultDto>(result);
            return View(model);
        }

        // POST: TestResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, UpdateTestResultDto resultDto)
        {
            if (id != resultDto.TestResultId) return NotFound();
            if (ModelState.IsValid)
            {
                var originalResult = await _resultService.GetResultByIdAsync(id);
                if (originalResult == null) return NotFound();

                await _resultService.UpdateResultAsync(resultDto);
                return RedirectToAction("Index", new { testId = originalResult.TestId });
            }
            return View(resultDto);
        }

        // GET: TestResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();
            var result = await _resultService.GetResultByIdAsync(id.Value);
            if (result == null) return NotFound();

            ViewBag.TestId = result.TestId;

            return View(result);
        }

        // POST: TestResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id, int testId)
        {
            await _resultService.DeleteResultAsync(id);
            return RedirectToAction("Index", "TestResults", new { testId = testId });
        }
    }
}