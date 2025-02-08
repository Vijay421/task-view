using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.DAL;

public class TaskViewContext : IdentityDbContext<User>
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectCollaboration> ProjectCollaborations { get; set; }
    public DbSet<TaskList> Lists { get; set; }
    public DbSet<TaskItem> Items { get; set; }

    // This constructor must only be used in tests!
    public TaskViewContext()
    {}

    public TaskViewContext(DbContextOptions<TaskViewContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskItem>()
            .HasOne(i => i.SuperItem)
            .WithMany(i => i.SubItems)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<TaskItem>()
            .HasOne(i => i.Creator)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Project>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.Projects)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
            .HasMany(u => u.JoinedProjects)
            .WithMany(p => p.JoinedUsers)
            .UsingEntity<ProjectCollaboration>();

        // The project names have to be unique for each user.
        builder.Entity<Project>(b => b.HasIndex(p => new { p.CreatorId, p.Name }).IsUnique());
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
