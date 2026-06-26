using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Queries.MerchantGetOrders;

public class MerchantGetOrdersHandler(IRepository<OrderEntity> orderRepository)
    : IQueryHandler<MerchantGetOrdersQuery, ErrorOr<PagedResponse<MerchantOrderSummaryDto>>>
{
    public async Task<ErrorOr<PagedResponse<MerchantOrderSummaryDto>>> Handle(
        MerchantGetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await orderRepository.GetPagedAsync(
            _ => true,
            request.PageNumber,
            request.PageSize,
            trackChanges: false,
            cancellationToken,
            q => q.OrderByDescending(o => o.CreatedAt),
            o => o.Items,
            o => o.Customer);

        var items = orders.Select(o => new MerchantOrderSummaryDto(
            o.Id,
            o.OrderNumber,
            o.Status,
            o.Total,
            o.Items.Count,
            $"{o.Customer.FirstName} {o.Customer.LastName}",
            o.CreatedAt));

        return new PagedResponse<MerchantOrderSummaryDto>(
            items, totalCount, request.PageNumber, request.PageSize);
    }
}
