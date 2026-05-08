using SmartBusinessManager.Core.Models;

namespace SmartBusinessManager.Core.Services;

public interface IAuthService
{
    Task<ApiResponse<AppUser>> SignInAsync(string email, string password);
    Task<ApiResponse<AppUser>> SignUpAsync(string email, string password, string fullName);
    Task                       SignOutAsync();
}
