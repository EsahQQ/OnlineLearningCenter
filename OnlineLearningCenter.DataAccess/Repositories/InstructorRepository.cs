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
    public async Task<(List<Instructor> Items, int TotalCount)> GetPaginatedInstructorsAsync(string? searchString, int pageNumber, int pageSize)
    {
        var query = _context.Instructors.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(i => i.FullName.Contains(searchString));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(i => i.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }
}