using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Data;

/// <summary>
/// Represents the Entity Framework Core database context for the TodoListDb database.
/// </summary>
public class TodoListDbContext : IdentityDbContext<IdentityUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the to-do lists table.</summary>
    public DbSet<TodoListEntity> TodoLists { get; set; }

    /// <summary>Gets or sets the to-do tasks table.</summary>
    public DbSet<TodoTaskEntity> TodoTasks { get; set; }

    /// <summary>Gets or sets the to-do tasks table.</summary>
    public DbSet<TodoTaskCommentEntity> Comments { get; set; }

    /// <summary>Gets or sets the tags table.</summary>
    public DbSet<TagEntity> Tags { get; set; }

    /// <summary>Gets or sets the TodoListAccess table.</summary>
    public DbSet<TodoListAccessEntity> TodoListAccess { get; set; }

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TodoTaskEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.AssignedTo).HasMaxLength(450);

            entity.HasIndex(e => e.AssignedTo);

            entity.HasOne(e => e.TodoList)
                  .WithMany(l => l.Tasks)
                  .HasForeignKey(e => e.TodoListId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TagEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        builder.Entity<TodoTaskCommentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.HasOne(e => e.TodoTask)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(e => e.TodoTaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
