using AutoMapper;
using OnlineLearningCenter.BusinessLogic.DTOs;
using OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.BusinessLogic.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Instructor, InstructorDto>();

            CreateMap<CreateCourseDto, Course>();

            CreateMap<Course, CourseDto>()
                .ForMember(
                    dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Instructor.FullName)
                )
                .ForMember(dest => dest.InstructorId,
                    opt => opt.MapFrom(src => src.InstructorId)
                );

            CreateMap<CourseDto, CreateCourseDto>();
        }
    }
}