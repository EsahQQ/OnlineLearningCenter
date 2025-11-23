using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.DTOs
{
    public class StudentRankingDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}
