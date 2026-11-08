using WPFW_opdracht.Dtos;

namespace WPFW_opdracht.Services;

public interface IBlogPost
{
    Task<List<BlogPostResponse>> GetAllAsync();
    Task<BlogPostResponse?> GetByIdAsync(int id);
    Task<BlogPostResponse> CreateAsync(CreateBlogPostRequest request);
    Task<bool> UpdateAsync(int id, UpdateBlogPostRequest request);
    Task<bool> DeleteAsync(int id);
}
