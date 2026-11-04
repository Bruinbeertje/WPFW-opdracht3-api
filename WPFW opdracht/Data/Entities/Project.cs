namespace WPFW_opdracht.Data.Entities;
    public class Project
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? GitHubUrl { get; set; } // ? operator laat null toe, heb ik gedaan omdat niet elk project een GitHubUrl heeft (vanwege privacy)
        public DateTime Date { get; set; }
    }