using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;

public class RegisterMerchantHandler(
    IUserService userService,
    IRepository<Merchant> repository)
    : ICommandHandler<RegisterMerchantCommand, ErrorOr<MerchantDto>>
{
    public async Task<ErrorOr<MerchantDto>> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
    {
        await repository.BeginTransactionAsync();
        
        try
        {
            var existingUser = await userService.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return AuthErrors.EmailAlreadyRegistered;

            var existingMerchant = await repository.FindAsync(m => m.Slug == request.Slug, trackChanges: false);
            if (existingMerchant.Any())
                return MerchantErrors.SlugTaken;

            var user = await userService.CreateUserAsync(request.Email, request.Password, "Merchant");
            if (user is null)
                return AuthErrors.IdentityCreationFailed;

            var merchant = new Merchant
            {
                ApplicationUserId = user.Id,
                StoreName = request.StoreName,
                Slug = request.Slug,
                Description = request.Description,
                LogoUrl = request.LogoUrl
            };

            await repository.AddAsync(merchant);
            await repository.SaveChangesAsync();
            await repository.CommitTransactionAsync();

            return new MerchantDto(merchant.Id, merchant.StoreName, merchant.Slug, merchant.Description, merchant.LogoUrl);
        }
        catch
        {
            await repository.RollbackTransactionAsync();
            throw;
        }
    }
}
