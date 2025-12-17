using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services.Interfaces;

public interface ICertificateService
{
    Task<IEnumerable<CertificateDto>> GetCertificatesByStudentIdAsync(int studentId);
    Task<CertificateDto?> GetCertificateByIdAsync(int certificateId);
    Task<CertificateDto> CreateCertificateAsync(CreateCertificateDto certificateDto);
    Task UpdateCertificateAsync(UpdateCertificateDto certificateDto);
    Task DeleteCertificateAsync(int certificateId);
}