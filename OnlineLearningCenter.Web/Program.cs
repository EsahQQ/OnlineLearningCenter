using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.BusinessLogic.Mappings;
using OnlineLearningCenter.BusinessLogic.Services;
using OnlineLearningCenter.DataAccess.Data;
using OnlineLearningCenter.DataAccess.Interfaces;
using OnlineLearningCenter.DataAccess.Repositories;

namespace OnlineLearningCenter.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Service Registration
            IConfigurationRoot configuration = builder.Configuration.AddUserSecrets<Program>().Build();

            var appConnectionString = configuration.GetConnectionString("AppConnection");
            string secretPass = configuration["Database:password"];
            string secretUser = configuration["Database:login"];
            var appSqlBuilder = new SqlConnectionStringBuilder(appConnectionString)
            {
                UserID = builder.Configuration["Database:AppUser"],
                Password = builder.Configuration["Database:AppPassword"]
            };

            var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
            var identitySqlBuilder = new SqlConnectionStringBuilder(identityConnectionString)
            {
                UserID = builder.Configuration["Database:IdentityUser"],
                Password = builder.Configuration["Database:IdentityPassword"]
            };

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appConnectionString));
            builder.Services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(identityConnectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

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
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IModuleService, ModuleService>();
            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<ITestResultService, TestResultService>();
            builder.Services.AddScoped<ICertificateService, CertificateService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

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