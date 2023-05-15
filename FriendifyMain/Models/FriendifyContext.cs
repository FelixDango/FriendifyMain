using FriendifyMain.Models;
using Microsoft.EntityFrameworkCore;

public class FriendifyContext : DbContext
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
            .HasOne(c => c.User) // specify the navigation property
            .WithMany(p => p.Images) // specify the inverse navigation property
            .HasForeignKey(c => c.UserId); // specify the foreign key property

        modelBuilder.Entity<Video>()
            .HasOne(c => c.User) // specify the navigation property
            .WithMany(p => p.Videos) // specify the inverse navigation property
            .HasForeignKey(c => c.UserId); // specify the foreign key property

        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages) // specify the navigation property
            .WithOne(m => m.User) // specify the inverse navigation property
            .HasForeignKey(m => m.UserId); // specify the foreign key property
    }
}