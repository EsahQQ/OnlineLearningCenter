using Microsoft.EntityFrameworkCore;
using OnlineLearningCenter.DataAccess.Entities;

namespace OnlineLearningCenter.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<Certificate> Certificates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Title)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<Certificate>()
            .HasIndex(c => c.CertificateUrl)
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();

        modelBuilder.Entity<Module>()
            .HasOne(m => m.Course)
            .WithMany(c => c.Modules)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
                .Property(e => e.Progress)
                .HasColumnType("decimal(5, 2)");

        modelBuilder.Entity<TestResult>()
                .ToTable(tb => tb.HasTrigger("TRG_UpdateEnrollmentProgress"));
    }
}