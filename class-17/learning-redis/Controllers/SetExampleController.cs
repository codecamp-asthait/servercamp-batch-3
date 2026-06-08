using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("set-example")]
public class SetExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();

    // Today's set key — auto-rotates daily, no cleanup needed
    private static string TodayKey => $"visitors:{DateTime.UtcNow:yyyy-MM-dd}";

    // POST /set-example/visit — add an IP to today's unique-visitor set
    [HttpPost("visit")]
    public IActionResult RecordVisit([FromBody] SetExampleVisitRequest request)
    {
        // SetAdd returns true if the member was new, false if already present.
        // Duplicate adds are silently ignored by Redis — no error, no overwrite.
        // Equivalent to: SADD visitors:2026-05-22 192.168.1.1
        bool isNew = _db.SetAdd(TodayKey, request.IpAddress);

        return Ok(new SetExampleVisitResponse(request.IpAddress, isNew));
    }

    // GET /set-example/unique-count — return the count of unique visitors today
    [HttpGet("unique-count")]
    public IActionResult GetUniqueCount()
    {
        // SetLength returns the cardinality of the set in O(1).
        // Returns 0 if the key doesn't exist — no null check needed.
        // Equivalent to: SCARD visitors:2026-05-22
        long count = _db.SetLength(TodayKey);

        return Ok(new SetExampleUniqueCountResponse(DateTime.UtcNow.ToString("yyyy-MM-dd"), count));
    }
}
