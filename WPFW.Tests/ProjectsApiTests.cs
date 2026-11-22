using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using WPFW_opdracht.Data.Entities;
using WPFW_opdracht.Dtos;

namespace WPFW.Tests;

public class ProjectsApiTests : IClassFixture<TestApiFactory>
{
    // kast
    private readonly TestApiFactory _factory;

    // xUnit geeft de factory en ik stop het in de kast
    public ProjectsApiTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateClientWithRoleAsync(string? role)
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.local";
        var password = $"Tt-{Guid.NewGuid()}!1";

        await client.PostAsJsonAsync("/api/Auth/register", new { email, password });

        if (role is not null)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, role);
        }
        await client.PostAsJsonAsync("/api/Auth/login", new { email, password });

        return client;
    }

    // [Fact]- en [Theory]-tests

    [Fact]
    public async Task Create_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nieuw = new { title = "Test", description = "Omschrijving", category = "Web", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task Create_AlsGast_Geeft403()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(null);
        var nieuw = new { title = "Test", description = "Omschrijving", category = "Web", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]      //ik test bijde rollen in 1 test
    public async Task Create_AlsSchrijver_Geeft201EnIsOpvraagbaar(string role)
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(role);
        var nieuw = new { title = "Mijn project", description = "Omschrijving", category = "Web", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var gemaakt = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        var opgehaald = await client.GetFromJsonAsync<ProjectResponse>($"/api/projects/{gemaakt!.Id}");
        Assert.Equal("Mijn project", opgehaald!.Title);   // effect+statuscode controleren
    }
}




