using Microsoft.EntityFrameworkCore;
using WPFW_opdracht.Data;
using WPFW_opdracht.Services;
namespace WPFW_opdracht;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IBlogPost, BlogPostService>();

        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<PortfolioDb>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}