using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Orders.Commands.UpdateOrderStatus;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Features.Orders.Queries.MerchantGetOrder;
using Dukaan.Application.Features.Orders.Queries.MerchantGetOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
[Route("api/merchant/orders")]
public class MerchantOrdersController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<MerchantOrderSummaryDto>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return ToActionResult(
            await Mediator.Send(new MerchantGetOrdersQuery(pageNumber, pageSize)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MerchantOrderDto>> GetOrder(Guid id)
    {
        return ToActionResult(
            await Mediator.Send(new MerchantGetOrderQuery(id)));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        return ToActionResult(
            await Mediator.Send(new UpdateOrderStatusCommand(id, request.NewStatus)));
    }
}

public record UpdateOrderStatusRequest(Domain.Enums.OrderStatus NewStatus);
