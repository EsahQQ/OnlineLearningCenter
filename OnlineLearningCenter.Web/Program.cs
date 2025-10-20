using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.BusinessLogic.Mappings;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Interfaces;
using OnlineLearningCenter.DataAccess.Repositories;
using Microsoft.Data.SqlClient;

namespace OnlineLearningCenter.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Service Registration
            IConfigurationRoot configuration = builder.Configuration.AddUserSecrets<Program>().Build();
            var connectionString = configuration.GetConnectionString("RemoteSQLConnection");
            string secretPass = configuration["Database:password"];
            string secretUser = configuration["Database:login"];
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                Password = secretPass,
                UserID = secretUser
            };
            connectionString = sqlConnectionStringBuilder.ConnectionString;

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
            builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
            builder.Services.AddScoped<ITestRepository, TestRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
            builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();

            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IInstructorService, InstructorService>();


            builder.Services.AddControllersWithViews();
            #endregion

            var app = builder.Build();

            // Настройка конвейера обработки HTTP-запросов (HTTP request pipeline)
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}