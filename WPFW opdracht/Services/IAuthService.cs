using Microsoft.AspNetCore.Identity;
using WPFW_opdracht.Dtos;

namespace WPFW_opdracht.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterUserRequest request);
    Task<SignInResult> LoginAsync(LoginUserRequest request);
    Task LogoutAsync();
}
