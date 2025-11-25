using OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.DataAccess.Interfaces;

public interface IInstructorRepository : IGenericRepository<Instructor> 
{
    Task<(List<Instructor> Items, int TotalCount)> GetPaginatedInstructorsAsync(string? searchString, int pageNumber, int pageSize);
}