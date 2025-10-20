using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineLearningCenter.DataAccess.Data;
using System.IO;

namespace OnlineLearningCenter.DataAccess
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OnlineLearningCenter.Web"))
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<ApplicationDbContextFactory>()
                .Build();

            var connectionStringTemplate = configuration.GetConnectionString("DefaultConnection");
            var login = configuration["Database:login"];
            var password = configuration["Database:password"];

            var fullConnectionString = $"{connectionStringTemplate}User Id={login};Password={password};";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(fullConnectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}