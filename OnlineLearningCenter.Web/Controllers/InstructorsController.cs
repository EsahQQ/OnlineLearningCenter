using AutoMapper; 
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers
{
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
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var instructors = await _instructorService.GetPaginatedInstructorsAsync(pageNumber);
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var instructor = await _instructorService.GetInstructorByIdAsync(id.Value);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _instructorService.DeleteInstructorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}