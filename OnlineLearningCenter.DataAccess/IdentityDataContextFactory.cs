using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineLearningCenter.DataAccess.Data;
using System.IO;

namespace OnlineLearningCenter.DataAccess;

public class IdentityDataContextFactory : IDesignTimeDbContextFactory<IdentityDataContext>
{
    public IdentityDataContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OnlineLearningCenter.Web"))
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<IdentityDataContextFactory>()
            .Build();

        var connectionStringTemplate = configuration.GetConnectionString("IdentityConnection");
        var login = configuration["Database:IdentityUser"];
        var password = configuration["Database:IdentityPassword"];

        var fullConnectionString = $"{connectionStringTemplate}User Id={login};Password={password};";

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDataContext>();
        optionsBuilder.UseSqlServer(fullConnectionString);

        return new IdentityDataContext(optionsBuilder.Options);
    }
}