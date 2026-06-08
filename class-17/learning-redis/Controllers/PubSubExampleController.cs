using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("pubsub-example")]
public class PubSubExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private const string Channel = "system-notifications";

    // POST /pubsub-example/notify — publish a message to the system-notifications channel
    [HttpPost("notify")]
    public async Task<IActionResult> Notify([FromBody] PubSubExampleNotifyRequest request)
    {
        // GetSubscriber() returns the pub/sub multiplexer — not IDatabase.
        // PublishAsync sends to all active subscribers on the channel.
        var sub = redis.GetSubscriber();
        await sub.PublishAsync(RedisChannel.Literal(Channel), request.Message);

        return Ok(new PubSubExampleNotifyResponse(Channel, request.Message));
    }
}
