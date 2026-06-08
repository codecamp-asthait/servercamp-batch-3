using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("list-example")]
public class ListExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();

    // POST /list-example/search — prepend a search term to the user's history
    [HttpPost("search")]
    public IActionResult AddSearch([FromBody] ListExampleSearchRequest request)
    {
        var key = $"search-history:{request.UserId}";

        // ListLeftPush prepends — newest item is always at index 0.
        // Equivalent to: LPUSH search-history:user:1 "redis lists"
        _db.ListLeftPush(key, request.Term);

        // ListTrim keeps only indices 0–9, dropping anything older.
        // Equivalent to: LTRIM search-history:user:1 0 9
        _db.ListTrim(key, 0, 9);

        return Ok(new ListExampleSearchResponse(request.UserId, request.Term));
    }

    // GET /list-example/search/history?userId=user:1 — retrieve the 5 most recent searches
    [HttpGet("search/history")]
    public IActionResult GetHistory([FromQuery] string userId)
    {
        var key = $"search-history:{userId}";

        // ListRange reads indices 0–4 without removing them.
        // Safe on empty or short lists — returns what exists, no error.
        // Equivalent to: LRANGE search-history:user:1 0 4
        var items = _db.ListRange(key, 0, 4);

        return Ok(new ListExampleHistoryResponse(userId, items.Select(x => x.ToString()).ToArray()));
    }
}
