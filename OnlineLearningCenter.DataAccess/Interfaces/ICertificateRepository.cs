using OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface ICertificateRepository : IGenericRepository<Certificate> 
{
    Task<IEnumerable<Certificate>> GetCertificatesByStudentIdWithDetailsAsync(int studentId);
    Task<Certificate?> GetCertificateByIdWithDetailsAsync(int certificateId);
}