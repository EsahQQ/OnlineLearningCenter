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
            var appSqlBuilder = new SqlConnectionStringBuilder(appConnectionString)
            {
                UserID = configuration["Database:login"],
                Password = configuration["Database:password"]
            };

            var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
            var identitySqlBuilder = new SqlConnectionStringBuilder(identityConnectionString)
            {
                UserID = configuration["Database:IdentityUser"],
                Password = configuration["Database:IdentityPassword"]
            };

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appSqlBuilder.ConnectionString));
            builder.Services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(identitySqlBuilder.ConnectionString));

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

            builder.Services.AddDistributedMemoryCache(); 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; 
            });
            #endregion

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var seedTask = Data.IdentityDataInitializer.SeedRolesAndAdminUser(serviceProvider);
                seedTask.Wait();
            }

            // Настройка конвейера обработки HTTP-запросов
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseSession();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}