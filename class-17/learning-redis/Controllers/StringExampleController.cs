using System.Text.Json;
using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("string-example")]
public class StringExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    // POST /string-example — store any JSON value in Redis with a TTL
    [HttpPost]
    public IActionResult Set(StringExampleRequest request)
    {
        var db = redis.GetDatabase();

        // Serialize the JsonElement back to a JSON string for storage.
        // Redis Strings hold raw bytes — we store JSON text.
        var json = JsonSerializer.Serialize(request.Value);

        // StringSet stores the key-value pair.
        // The TimeSpan argument tells Redis to automatically delete
        // this key after the specified duration — this is the TTL.
        db.StringSet(request.Key, json, TimeSpan.FromSeconds(request.TtlSeconds));

        return CreatedAtAction(nameof(Get), new { key = request.Key }, new
        {
            request.Key,
            ExpiresInSeconds = request.TtlSeconds
        });
    }

    // GET /cache/{key} — retrieve a cached value by key
    [HttpGet("{key}")]
    public IActionResult Get(string key)
    {
        var db = redis.GetDatabase();

        // StringGet returns RedisValue.Null if the key doesn't exist
        // (either it was never set, or the TTL expired and Redis deleted it)
        var raw = db.StringGet(key);

        // Cache miss — key is gone
        if (raw.IsNullOrEmpty)
            return NotFound(new { message = "Key not found or expired." });

        // Cache hit — deserialize back to a JsonElement so we return proper JSON,
        // not a double-encoded string
        var value = JsonSerializer.Deserialize<JsonElement>((string)raw!);

        return Ok(new { key, value });
    }
}
