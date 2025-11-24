using System.ComponentModel.DataAnnotations;
namespace OnlineLearningCenter.BusinessLogic.DTOs;

public class CreateCertificateDto
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int CourseId { get; set; }
    [Required, Url]
    [Display(Name = "Ссылка на сертификат")]
    public string CertificateUrl { get; set; } = string.Empty;
}