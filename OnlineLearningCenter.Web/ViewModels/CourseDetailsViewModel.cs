using OnlineLearningCenter.BusinessLogic.DTOs;

namespace OnlineLearningCenter.Web.ViewModels
{
    public class CourseDetailsViewModel
    {
        public CourseDto Course { get; set; }

        public CourseAnalyticsDto? Analytics { get; set; }
    }
}
