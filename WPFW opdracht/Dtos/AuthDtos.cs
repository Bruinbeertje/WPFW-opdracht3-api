using System.ComponentModel.DataAnnotations;

namespace WPFW_opdracht.Dtos;

public class RegisterUserRequest
{
    [Required][EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}

public class LoginUserRequest
{
    [Required][EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}

