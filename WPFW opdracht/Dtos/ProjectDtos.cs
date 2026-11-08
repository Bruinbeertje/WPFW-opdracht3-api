using System.ComponentModel.DataAnnotations;

namespace WPFW_opdracht.Dtos;

public class CreateProjectRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Url]
    public string? GitHubUrl { get; set; }

    public DateTime Date { get; set; }
}

public class UpdateProjectRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Url]
    public string? GitHubUrl { get; set; }

    public DateTime Date { get; set; }
}

public class ProjectResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? GitHubUrl { get; set; }

    public DateTime Date { get; set; }
}