using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;
    private readonly ICertificateService _certificateService;

    public StudentsController(IStudentService studentService, IMapper mapper, ICertificateService certificateService)
    {
        _studentService = studentService;
        _mapper = mapper;
        _certificateService = certificateService;
    }

    // GET: Students
    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
    {
        var students = await _studentService.GetPaginatedStudentsAsync(searchString,pageNumber);

        ViewData["CurrentFilter"] = searchString;

        return View(students);
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var student = await _studentService.GetStudentByIdAsync(id.Value);
        if (student == null) return NotFound();

        var studentProgress = await _studentService.GetStudentProgressAsync(id.Value);

        var certificates = await _certificateService.GetCertificatesByStudentIdAsync(id.Value);
        ViewBag.Certificates = certificates;

        ViewBag.Student = student;
        return View(studentProgress);
    }

    // GET: Students/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateStudentDto studentDto)
    {
        if (ModelState.IsValid)
        {
            await _studentService.CreateStudentAsync(studentDto);
            return RedirectToAction(nameof(Index));
        }
        return View(studentDto);
    }

    // GET: Students/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var student = await _studentService.GetStudentByIdAsync(id.Value);
        if (student == null) return NotFound();

        var studentToUpdate = _mapper.Map<UpdateStudentDto>(student);
        return View(studentToUpdate);
    }

    // POST: Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, UpdateStudentDto studentDto)
    {
        if (id != studentDto.StudentId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _studentService.UpdateStudentAsync(studentDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(studentDto);
    }

    // GET: Students/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var student = await _studentService.GetStudentByIdAsync(id.Value);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return RedirectToAction(nameof(Index));
    }
}