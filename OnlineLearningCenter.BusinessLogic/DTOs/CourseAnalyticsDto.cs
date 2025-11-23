using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningCenter.BusinessLogic.DTOs
{
    public class CourseAnalyticsDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int StudentsCompleted { get; set; }
        public int TotalStudentsEnrolled { get; set; }
        public double AverageScoreForCourse { get; set; }
    }
}
