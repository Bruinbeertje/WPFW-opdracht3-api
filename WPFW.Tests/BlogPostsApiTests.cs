using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using WPFW_opdracht.Data.Entities;
using WPFW_opdracht.Dtos;

namespace WPFW.Tests;

public class BlogPostsApiTests : IClassFixture<TestApiFactory>
{
    // kast
    private readonly TestApiFactory _factory;

    // xUnit geeft de factory en ik stop het in de kast
    public BlogPostsApiTests(TestApiFactory factory)
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

    private async Task<BlogPostResponse> CreateBlogPostAsync(HttpClient client)
    {
        var nieuw = new { title = "Mijn blogpost", content = "Inhoud van de blogpost", date = DateTime.Today };
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);
        return (await response.Content.ReadFromJsonAsync<BlogPostResponse>())!;
    }

    // [Fact]- en [Theory]-tests

    [Fact]
    public async Task Create_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nieuw = new { title = "Test", content = "Inhoud van de blogpost", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task Create_AlsGast_Geeft403()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(null);
        var nieuw = new { title = "Test", content = "Inhoud van de blogpost", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]      //ik test beide rollen in 1 test
    public async Task Create_AlsSchrijver_Geeft201EnIsOpvraagbaar(string role)
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(role);
        var nieuw = new { title = "Mijn blogpost", content = "Inhoud van de blogpost", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var gemaakt = await response.Content.ReadFromJsonAsync<BlogPostResponse>();
        var opgehaald = await client.GetFromJsonAsync<BlogPostResponse>($"/api/blogposts/{gemaakt!.Id}");
        Assert.Equal("Mijn blogpost", opgehaald!.Title);   // effect+statuscode controleren
    }

    [Fact]
    public async Task GetAll_ZonderLogin_Geeft200EnBevatNieuwBlogPost()
    {
        // Arrange
        var editor = await CreateClientWithRoleAsync("Editor");
        var gemaakt = await CreateBlogPostAsync(editor);
        var anoniem = _factory.CreateClient();

        // Act
        var response = await anoniem.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogPosts = await response.Content.ReadFromJsonAsync<List<BlogPostResponse>>();
        Assert.Contains(blogPosts!, blogPost => blogPost.Id == gemaakt.Id);
    }

    [Fact]
    public async Task GetById_ZonderLogin_Geeft200EnJuistBlogPost()
    {
        // Arrange
        var editor = await CreateClientWithRoleAsync("Editor");
        var gemaakt = await CreateBlogPostAsync(editor);
        var anoniem = _factory.CreateClient();

        // Act
        var response = await anoniem.GetAsync($"/api/blogposts/{gemaakt.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var opgehaald = await response.Content.ReadFromJsonAsync<BlogPostResponse>();
        Assert.Equal("Mijn blogpost", opgehaald!.Title);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_OngeldigeRequest_Geeft400()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync("Editor");
        var nieuw = new { title = "", content = "Inhoud van de blogpost", date = DateTime.Today };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Update_AlsSchrijver_Geeft204EnWijzigtBlogPost(string role)
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(role);
        var gemaakt = await CreateBlogPostAsync(client);
        var gewijzigd = new { title = "Gewijzigde blogpost", content = "Nieuwe inhoud", date = DateTime.Today };

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{gemaakt.Id}", gewijzigd);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<BlogPostResponse>($"/api/blogposts/{gemaakt.Id}");
        Assert.Equal("Gewijzigde blogpost", opgehaald!.Title);   // effect+statuscode controleren
    }

    [Fact]
    public async Task Update_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync("Editor");
        var gewijzigd = new { title = "Gewijzigde blogpost", content = "Nieuwe inhoud", date = DateTime.Today };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/999999", gewijzigd);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_OngeldigeRequest_Geeft400()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync("Editor");
        var gewijzigd = new { title = "", content = "Nieuwe inhoud", date = DateTime.Today };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/1", gewijzigd);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task Delete_AlsSchrijver_Geeft204EnDaarna404(string role)
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(role);
        var gemaakt = await CreateBlogPostAsync(client);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{gemaakt.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetAsync($"/api/blogposts/{gemaakt.Id}");
        Assert.Equal(HttpStatusCode.NotFound, opgehaald.StatusCode);   // effect+statuscode controleren
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync("Editor");

        // Act
        var response = await client.DeleteAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gewijzigd = new { title = "Gewijzigde blogpost", content = "Nieuwe inhoud", date = DateTime.Today };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/1", gewijzigd);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ZonderLogin_Geeft401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/blogposts/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_AlsGast_Geeft403()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(null);
        var gewijzigd = new { title = "Gewijzigde blogpost", content = "Nieuwe inhoud", date = DateTime.Today };

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/1", gewijzigd);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsGast_Geeft403()
    {
        // Arrange
        var client = await CreateClientWithRoleAsync(null);

        // Act
        var response = await client.DeleteAsync("/api/blogposts/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
