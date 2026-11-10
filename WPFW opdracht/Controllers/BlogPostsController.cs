using Microsoft.AspNetCore.Mvc;
using WPFW_opdracht.Dtos;
using WPFW_opdracht.Services;

namespace WPFW_opdracht.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _service;

    public BlogPostsController(IBlogPostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<BlogPostResponse>>> GetAll()
    {
        var blogPosts = await _service.GetAllAsync();
        return Ok(blogPosts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogPostResponse>> GetById(int id)
    {
        var blogPost = await _service.GetByIdAsync(id);

        if (blogPost is null)
        {
            return NotFound();
        }

        return Ok(blogPost);
    }

    [HttpPost]
    public async Task<ActionResult<BlogPostResponse>> Create(CreateBlogPostRequest request)
    {
        var blogPost = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = blogPost.Id }, blogPost);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBlogPostRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
