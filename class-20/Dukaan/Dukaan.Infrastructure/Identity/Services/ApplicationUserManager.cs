using Dukaan.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Dukaan.Infrastructure.Identity.Interfaces;

namespace Dukaan.Infrastructure.Identity.Services;

public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
{
    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}