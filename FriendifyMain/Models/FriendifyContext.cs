using FriendifyMain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FriendifyContext : IdentityDbContext<User, Role, int>
{
    public FriendifyContext(DbContextOptions<FriendifyContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<AssignedRole> AssignedRoles { get; set; }
    // Add other DbSets as needed

    // Override the OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // configure the base class for IdentityUser
        base.OnModelCreating(modelBuilder);

        // configure the one-to-many relationship between User and Post
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts) // specify the navigation property
            .WithOne(p => p.User) // specify the inverse navigation property
            .HasForeignKey(p => p.UserId); // specify the foreign key property

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post) // specify the navigation property
            .WithMany(p => p.Comments) // specify the inverse navigation property
            .HasForeignKey(c => c.PostId); // specify the foreign key property

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User) // specify the navigation property
            .WithMany(p => p.Comments) // specify the inverse navigation property
            .HasForeignKey(c => c.UserId); // specify the foreign key property

        modelBuilder.Entity<Picture>()
            .HasOne(p => p.User)
            .WithMany(u => u.Images)
            .HasForeignKey(p => p.UserId)
            .HasPrincipalKey(u => u.Id);

        modelBuilder.Entity<Video>()
            .HasOne(c => c.User) // specify the navigation property
            .WithMany(p => p.Videos) // specify the inverse navigation property
            .HasForeignKey(c => c.UserId); // specify the foreign key property

        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages) // specify the navigation property
            .WithOne(m => m.User) // specify the inverse navigation property
            .HasForeignKey(m => m.UserId) // specify the foreign key property
            .OnDelete(DeleteBehavior.NoAction); // Prevent cascading delete

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}