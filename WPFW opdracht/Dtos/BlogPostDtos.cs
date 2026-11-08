using System.ComponentModel.DataAnnotations;


namespace WPFW_opdracht.Dtos;

public class CreateBlogPostRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }

}

public class UpdateBlogPostRequest
{

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }

}

public class BlogPostResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }

}
