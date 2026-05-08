using SmartBusinessManager.Core.Models;

namespace SmartBusinessManager.Core.Services;

public class AuthService : IAuthService
{
    public Task<ApiResponse<AppUser>> SignInAsync(string email, string password) => throw new NotImplementedException();
    public Task<ApiResponse<AppUser>> SignUpAsync(string email, string password, string fullName) => throw new NotImplementedException();
    public Task SignOutAsync() => throw new NotImplementedException();
}
