

using courses.Models;
using Microsoft.EntityFrameworkCore;

namespace courses.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Course> Courses { get; set; }

    public DbSet<Student> Students { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
           {
               entity.HasKey(entity => entity.Id);
               entity.Property(entity => entity.Name).IsRequired().HasMaxLength(50);
           });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Course)
                .WithMany(e => e.Students)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
