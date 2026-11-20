using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WPFW_opdracht.Data.Entities;
using WPFW_opdracht.Data;
using WPFW_opdracht.Services;
namespace WPFW_opdracht;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IBlogPostService, BlogPostService>();

        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("CanEdit", policy => policy.RequireRole("Editor", "Admin"));  //betekent: de CanEdit policy kan alleen uitgevoerd worden door een user met de rol Editor of Admin

        builder.Services.AddDbContext<PortfolioDb>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentityApiEndpoints<User>() // Identity & cookie
            .AddRoles<IdentityRole>() // Rollen aanzetten
            .AddEntityFrameworkStores<PortfolioDb>(); // opslaan in de database

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDb>();
            await db.Database.MigrateAsync(); // database bijwerken voor testen

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { "User", "Editor", "Admin" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

        app.UseHttpsRedirection();

        app.UseAuthentication(); // wie ben je
        app.UseAuthorization(); // mag je dit doen


        app.MapControllers();

        app.Run();
    }
}