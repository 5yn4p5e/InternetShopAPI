using InternetShop.InternetShopModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace InternetShop.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateAsync(User user, string password);
        Task AddToRoleAsync(User user, string role);
        Task SignInAsync(User user, bool isPersistent);
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe);
        Task<User> FindByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(User user);
        Task SignOutAsync();
        Task<User> GetUserAsync(ClaimsPrincipal principal);
    }
}
