using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
{
    public CertificateRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<Certificate?> GetCertificateByIdWithDetailsAsync(int certificateId)
    {
        return await _context.Certificates
            .Include(c => c.Student)
            .Include(c => c.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CertificateId == certificateId);
    }

    public async Task<IEnumerable<Certificate>> GetCertificatesByStudentIdWithDetailsAsync(int studentId)
    {
        return await _context.Certificates
            .Include(c => c.Student)
            .Include(c => c.Course)
            .Where(c => c.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();
    }
}