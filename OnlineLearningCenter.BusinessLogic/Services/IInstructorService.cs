using OnlineLearningCenter.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.Services;

public interface IInstructorService
{
    Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync();
}