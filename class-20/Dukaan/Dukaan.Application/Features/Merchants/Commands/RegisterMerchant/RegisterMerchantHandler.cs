using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Features.Auth;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Observability;

namespace Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;

public class RegisterMerchantHandler(
    IUserService userService,
    IRepository<Merchant> repository,
    IRepository<Tenant> tenantRepository)
    : ICommandHandler<RegisterMerchantCommand, ErrorOr<MerchantDto>>
{
    public async Task<ErrorOr<MerchantDto>> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
    {
        await repository.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var existingUser = await userService.FindByEmailAsync(request.Email);
            if (existingUser is not null) return AuthErrors.EmailAlreadyRegistered;

            var existingTenant = await tenantRepository.FindFirstAsync(t => t.Slug == request.Slug, trackChanges: false, cancellationToken: cancellationToken);
            if (existingTenant is not null) return MerchantErrors.SlugTaken;

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                StoreName = request.StoreName,
                Slug = request.Slug,
                Category = "General",
                Country = "BD",
                Currency = "BDT",
                CreatedAt = DateTime.UtcNow
            };
            await tenantRepository.AddAsync(tenant, cancellationToken);
            await tenantRepository.SaveChangesAsync(cancellationToken);

            var user = await userService.CreateUserAsync(request.Email, request.Password, "Merchant", tenant.Id);
            if (user is null) return AuthErrors.IdentityCreationFailed;


            var merchant = new Merchant
            {
                ApplicationUserId = user.Id,
                TenantId = tenant.Id,
            };

            await repository.AddAsync(merchant, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            await repository.CommitTransactionAsync(cancellationToken);

            DukaanMetrics.MerchantRegistrations.Add(1, DukaanMetrics.Tag("tenant_id", merchant.TenantId));

            return new MerchantDto(merchant.Id, tenant.StoreName, tenant.Slug);
        }
        catch
        {
            await repository.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
