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
                .ForMember(
                    dest => dest.InstructorId,
                    opt => opt.MapFrom(src => src.InstructorId)
                );

            CreateMap<CourseDto, CreateCourseDto>();

            CreateMap<CourseDto, UpdateCourseDto>();
            CreateMap<UpdateCourseDto, Course>();

            CreateMap<CreateInstructorDto, Instructor>();
            CreateMap<InstructorDto, UpdateInstructorDto>(); 
            CreateMap<UpdateInstructorDto, Instructor>();

            CreateMap<Student, StudentDto>()
                .ForMember(
                    dest => dest.RegistrationDate,
                    opt => opt.MapFrom(src => src.RegistrationDate.ToString("dd.MM.yyyy"))
                );

            CreateMap<CreateStudentDto, Student>();
            CreateMap<StudentDto, UpdateStudentDto>();
            CreateMap<UpdateStudentDto, Student>();

            CreateMap<Module, ModuleDto>();
            CreateMap<CreateModuleDto, Module>();
            CreateMap<ModuleDto, UpdateModuleDto>();
            CreateMap<UpdateModuleDto, Module>();

            CreateMap<Test, TestDto>();
            CreateMap<CreateTestDto, Test>();
            CreateMap<TestDto, UpdateTestDto>();
            CreateMap<UpdateTestDto, Test>();

            CreateMap<TestResult, TestResultDto>()
                .ForMember(dest => dest.StudentFullName,
                           opt => opt.MapFrom(src => src.Student.FullName)
                )
                .ForMember(dest => dest.TestTitle,
                           opt => opt.MapFrom(src => src.Test.Title)
                )
                .ForMember(dest => dest.TestId,
                           opt => opt.MapFrom(src => src.TestId)
                )
                .ForMember(dest => dest.TestResultId,
                           opt => opt.MapFrom(src => src.ResultId)
                )
                .ForMember(dest => dest.CompletionDate,
                           opt => opt.MapFrom(src => src.CompletionDate.ToString("dd.MM.yyyy"))
                );

            CreateMap<CreateTestResultDto, TestResult>()
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(System.DateTime.Now)));

            CreateMap<TestResultDto, UpdateTestResultDto>();

            CreateMap<Certificate, CertificateDto>()
                .ForMember(dest => dest.StudentFullName,
                           opt => opt.MapFrom(src => src.Student.FullName)
                )
                .ForMember(dest => dest.CourseTitle,
                           opt => opt.MapFrom(src => src.Course.Title)
                )
                .ForMember(dest => dest.IssueDate,
                           opt => opt.MapFrom(src => src.IssueDate.ToString("dd.MM.yyyy"))
                );

            CreateMap<CreateCertificateDto, Certificate>()
                .ForMember(dest => dest.IssueDate,
                           opt => opt.MapFrom(src => DateOnly.FromDateTime(System.DateTime.Now))
                );

            CreateMap<CertificateDto, UpdateCertificateDto>();
            CreateMap<UpdateCertificateDto, Certificate>();
        }
    }
}