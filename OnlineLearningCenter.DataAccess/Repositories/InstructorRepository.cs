using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class InstructorRepository : GenericRepository<Instructor>, IInstructorRepository
{
    public InstructorRepository(ApplicationDbContext context) : base(context)
    {
    }
    public IQueryable<Instructor> GetInstructorsQueryable()
    {
        return _context.Instructors.AsNoTracking();
    }
}