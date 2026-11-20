using Microsoft.EntityFrameworkCore;
using WPFW_opdracht.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WPFW_opdracht.Data;


public class PortfolioDb :IdentityDbContext<User>
{
    public PortfolioDb(DbContextOptions<PortfolioDb> options) : base(options) { }

public DbSet<Project> Projects { get; set; }
public DbSet<BlogPost> BlogPosts { get; set; }
}