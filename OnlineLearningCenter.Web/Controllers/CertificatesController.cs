using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.BusinessLogic.Services;
using System.Threading.Tasks;

namespace OnlineLearningCenter.Web.Controllers;

[Authorize]
public class CertificatesController : Controller
{
    private readonly ICertificateService _certificateService;
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public CertificatesController(ICertificateService certificateService, IStudentService studentService, ICourseService courseService, IMapper mapper)
    {
        _certificateService = certificateService;
        _studentService = studentService;
        _courseService = courseService;
        _mapper = mapper;
    }

    // GET: Certificates/Create
    public async Task<IActionResult> Create(int studentId, int courseId)
    {
        var student = await _studentService.GetStudentByIdAsync(studentId);
        var course = await _courseService.GetCourseByIdAsync(courseId);

        if (student == null || course == null) return NotFound();

        ViewBag.StudentName = student.FullName;
        ViewBag.CourseTitle = course.Title;

        var model = new CreateCertificateDto { StudentId = studentId, CourseId = courseId };
        return View(model);
    }

    // POST: Certificates/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCertificateDto certificateDto)
    {
        if (ModelState.IsValid)
        {
            await _certificateService.CreateCertificateAsync(certificateDto);
            return RedirectToAction("Details", "Students", new { id = certificateDto.StudentId });
        }

        var student = await _studentService.GetStudentByIdAsync(certificateDto.StudentId);
        var course = await _courseService.GetCourseByIdAsync(certificateDto.CourseId);
        ViewBag.StudentName = student?.FullName;
        ViewBag.CourseTitle = course?.Title;

        return View(certificateDto);
    }

    // GET: Certificates/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var certificate = await _certificateService.GetCertificateByIdAsync(id.Value);
        if (certificate == null) return NotFound();

        ViewBag.StudentName = certificate.StudentFullName;
        ViewBag.CourseTitle = certificate.CourseTitle;

        var model = _mapper.Map<UpdateCertificateDto>(certificate);
        return View(model);
    }

    // POST: Certificates/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, UpdateCertificateDto certificateDto)
    {
        if (id != certificateDto.CertificateId) return NotFound();

        if (ModelState.IsValid)
        {
            await _certificateService.UpdateCertificateAsync(certificateDto);
            return RedirectToAction("Details", "Students", new { id = certificateDto.StudentId });
        }

        var student = await _studentService.GetStudentByIdAsync(certificateDto.StudentId);
        var course = await _courseService.GetCourseByIdAsync(certificateDto.CourseId);
        ViewBag.StudentName = student?.FullName;
        ViewBag.CourseTitle = course?.Title;
        return View(certificateDto);
    }

    // GET: Certificates/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var certificate = await _certificateService.GetCertificateByIdAsync(id.Value);
        if (certificate == null) return NotFound();
        return View(certificate);
    }

    // POST: Certificates/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int certificateId, int studentId)
    {
        await _certificateService.DeleteCertificateAsync(certificateId);
        return RedirectToAction("Details", "Students", new { id = studentId });
    }
}