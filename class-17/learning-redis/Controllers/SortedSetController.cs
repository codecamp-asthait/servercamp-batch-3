using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("sorted-set-example")]
public class SortedSetExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string Key = "product-views";

    // POST /sorted-set-example/view — increment the view count for a product
    [HttpPost("view")]
    public IActionResult RecordView([FromBody] SortedSetExampleViewRequest request)
    {
        // SortedSetIncrement atomically adds 1 to the score.
        // Creates the member with score 1 if it doesn't exist yet.
        // Equivalent to: ZINCRBY product-views 1 product:42
        double newScore = _db.SortedSetIncrement(Key, request.ProductId, 1);

        return Ok(new SortedSetExampleViewResponse(request.ProductId, (long)newScore));
    }

    // GET /sorted-set-example/top — retrieve the top 5 most viewed products
    [HttpGet("top")]
    public IActionResult GetTop()
    {
        // SortedSetRangeByRankWithScores returns members with their scores.
        // Order.Descending = highest score first.
        // Equivalent to: ZREVRANGE product-views 0 4 WITHSCORES
        var top = _db.SortedSetRangeByRankWithScores(Key, 0, 4, Order.Descending);

        var leaderboard = top
            .Select(e => new SortedSetExampleLeaderboardEntry(e.Element.ToString(), (long)e.Score))
            .ToArray();

        return Ok(new SortedSetExampleTopResponse(leaderboard));
    }
}
