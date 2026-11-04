using Microsoft.EntityFrameworkCore;
namespace WPFW_opdracht.Data;
using WPFW_opdracht.Data.Entities;

public class PortfolioDb : DbContext
{
    public PortfolioDb(DbContextOptions<PortfolioDb> options) : base(options) { }

public DbSet<Project> Projects { get; set; }
public DbSet<BlogPost> BlogPosts { get; set; }
}