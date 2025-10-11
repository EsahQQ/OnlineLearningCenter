using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Entities;
using OnlineLearningCenter.DataAccess.Interfaces;

namespace OnlineLearningCenter.DataAccess.Repositories;

public class TestRepository : GenericRepository<Test>, ITestRepository
{
    public TestRepository(ApplicationDbContext context) : base(context)
    {
    }
}