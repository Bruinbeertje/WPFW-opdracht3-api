using Microsoft.EntityFrameworkCore;
using WPFW_opdracht.Data;
using WPFW_opdracht.Data.Entities;
using WPFW_opdracht.Dtos;

namespace WPFW_opdracht.Services;

public class ProjectService : IProjectService
{
    private readonly PortfolioDb _db;

    public ProjectService(PortfolioDb db)
    {
        _db = db;
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            GitHubUrl = request.GitHubUrl,
            Date = request.Date
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return ToResponse(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project is null)
        {
            return false;
        }
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        var projects = await _db.Projects.ToListAsync();
        return projects.Select(ToResponse).ToList();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project is null)
        {
            return null;
        }

        return ToResponse(project);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var project = await _db.Projects.FindAsync(id);

        if (project is null)
        {
            return false;
        }
        project.Title = request.Title;
        project.Description = request.Description;
        project.Category = request.Category;
        project.GitHubUrl = request.GitHubUrl;
        project.Date = request.Date;

        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
        return true;
    }
    private static ProjectResponse ToResponse(Project p) => new ProjectResponse
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Category = p.Category,
        GitHubUrl = p.GitHubUrl,
        Date = p.Date
    };
}


