using FriendifyMain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;

public class FriendifyContext : DbContext
{
    public FriendifyContext(DbContextOptions<FriendifyContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<AssignedRole> AssignedRoles { get; set; }
    // Add other DbSets as needed
}