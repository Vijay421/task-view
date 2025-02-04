using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.DAL;

public class TaskViewContext : IdentityDbContext<User>
{
    public DbSet<Project> Projects { get; set; }

    public TaskViewContext(DbContextOptions<TaskViewContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // The project names have to be unique for each user.
        builder.Entity<Project>(b => b.HasIndex(p => new { p.UserId, p.Name }).IsUnique());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) => {
            List<IdentityRole> roles = [
                new IdentityRole { Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "user", NormalizedName = "USER" },
            ];

            context.Set<IdentityRole>().AddRange(roles);
            context.SaveChanges();
        });
    }
}
