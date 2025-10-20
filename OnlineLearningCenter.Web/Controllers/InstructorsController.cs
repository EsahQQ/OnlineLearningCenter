using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IInstructorService _instructorService;

        public InstructorsController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        // GET: Instructors
        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorService.GetAllInstructorsAsync();

            return View(instructors);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            // var instructor = await _instructorService.GetInstructorByIdAsync(id.Value);
            // if (instructor == null) return NotFound();
            // return View(instructor);
            return Content($"Details for instructor {id} will be here.");
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            return Content("Create form for instructor will be here.");
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorDto instructor) 
        {
            return RedirectToAction(nameof(Index));
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            return Content($"Edit form for instructor {id} will be here.");
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorDto instructor) 
        {
            return RedirectToAction(nameof(Index));
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            return Content($"Delete confirmation for instructor {id} will be here.");
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}