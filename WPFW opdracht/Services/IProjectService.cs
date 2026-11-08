using WPFW_opdracht.Dtos;

namespace WPFW_opdracht.Services;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<bool> UpdateAsync(int id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(int id);
}
