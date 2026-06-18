using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;
using Dukaan.Infrastructure.Identity.Interfaces;

namespace Dukaan.Infrastructure.Identity.Adapters;

public class ApplicationUserManagerAdapter(IApplicationUserManager applicationUserManager) : IApplicationUserManagerAdapter
{

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await applicationUserManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindByIdAsync(string id)
    {
        return await applicationUserManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return await applicationUserManager.FindByNameAsync(userName);
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await applicationUserManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return await applicationUserManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
    {
        return await applicationUserManager.AddToRoleAsync(user, role);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await applicationUserManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {
        return await applicationUserManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        return await applicationUserManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return await applicationUserManager.GenerateEmailConfirmationTokenAsync(user);
    }
}