using Microsoft.EntityFrameworkCore;
using WPFW_opdracht.Data;
using WPFW_opdracht.Data.Entities;
using WPFW_opdracht.Dtos;

namespace WPFW_opdracht.Services;

public class BlogPostService : IBlogPost
{
    private readonly PortfolioDb _db;

    public BlogPostService(PortfolioDb db)
    {
        _db = db;
    }

    public async Task<BlogPostResponse> CreateAsync(CreateBlogPostRequest request)
    {
        var blogPost = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            Date = request.Date
        };

        _db.BlogPosts.Add(blogPost);
        await _db.SaveChangesAsync();

        return ToResponse(blogPost);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogPost = await _db.BlogPosts.FindAsync(id);

        if (blogPost is null)
        {
            return false;
        }

        _db.BlogPosts.Remove(blogPost);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<BlogPostResponse>> GetAllAsync()
    {
        var blogPosts = await _db.BlogPosts.ToListAsync();
        return blogPosts.Select(ToResponse).ToList();
    }

    public async Task<BlogPostResponse?> GetByIdAsync(int id)
    {
        var blogPost = await _db.BlogPosts.FindAsync(id);

        if (blogPost is null)
        {
            return null;
        }

        return ToResponse(blogPost);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBlogPostRequest request)
    {
        var blogPost = await _db.BlogPosts.FindAsync(id);

        if (blogPost is null)
        {
            return false;
        }

        blogPost.Title = request.Title;
        blogPost.Content = request.Content;
        blogPost.Date = request.Date;

        _db.BlogPosts.Update(blogPost);
        await _db.SaveChangesAsync();
        return true;
    }

    private static BlogPostResponse ToResponse(BlogPost blogPost) => new()
    {
        Id = blogPost.Id,
        Title = blogPost.Title,
        Content = blogPost.Content,
        Date = blogPost.Date
    };
}
