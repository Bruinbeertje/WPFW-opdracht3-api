using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using WPFW_opdracht.Data.Entities;

namespace WPFW.Tests;

public class AuthApiTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;

    public AuthApiTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Geldig_Geeft200EnRolGast()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.local";
        var password = $"Tt-{Guid.NewGuid()}!1";

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", new { email, password });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user!, "Gast"));
    }

    [Fact]
    public async Task Register_BestaandEmail_Geeft400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.local";
        var password = $"Tt-{Guid.NewGuid()}!1";
        var nieuw = new { email, password };
        await client.PostAsJsonAsync("/api/Auth/register", nieuw);

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("DuplicateUserName", body);
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nieuw = new { email = $"{Guid.NewGuid()}@test.local", password = "123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("PasswordTooShort", body);
    }

    [Fact]
    public async Task Login_JuistWachtwoord_Geeft200EnCookie()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.local";
        var password = $"Tt-{Guid.NewGuid()}!1";
        await client.PostAsJsonAsync("/api/Auth/register", new { email, password });

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", new { email, password });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));   // cookie is meegegeven
    }

    [Fact]
    public async Task Login_FoutWachtwoord_Geeft401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.local";
        var password = $"Tt-{Guid.NewGuid()}!1";
        await client.PostAsJsonAsync("/api/Auth/register", new { email, password });

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", new { email, password = "Verkeerd123!" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
}
