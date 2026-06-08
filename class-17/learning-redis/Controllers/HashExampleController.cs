using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("hash-example")]
public class HashExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();

    // POST /hash-example — create a user profile stored as a Redis Hash
    [HttpPost]
    public IActionResult Create([FromBody] HashExampleCreateRequest request)
    {
        var key = $"user:{request.Id}";

        // HashSet with a HashEntry array writes all fields in one round-trip.
        // Equivalent to: HSET user:1 name Alice email alice@example.com lastLogin ""
        _db.HashSet(key, [
            new HashEntry("name",      request.Name),
            new HashEntry("email",     request.Email),
            new HashEntry("lastLogin", string.Empty)
        ]);

        return CreatedAtAction(nameof(GetById), new { id = request.Id },
            new HashExampleProfileResponse(request.Id, request.Name, request.Email, null));
    }

    // GET /hash-example/{id} — retrieve the full user profile from the Hash
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var key = $"user:{id}";

        // KeyExists is a cheap O(1) check before reading all fields
        if (!_db.KeyExists(key))
            return NotFound(new { message = "User not found." });

        // HashGetAll returns all field-value pairs in one round-trip
        var entries = _db.HashGetAll(key).ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());

        var lastLogin = entries["lastLogin"];

        return Ok(new HashExampleProfileResponse(
            Id: id,
            Name: entries["name"],
            Email: entries["email"],
            LastLogin: string.IsNullOrEmpty(lastLogin) ? null : lastLogin
        ));
    }

    // PATCH /hash-example/{id}/last-login — update only the lastLogin field
    // Demonstrates the core advantage of Hashes: field-level writes
    [HttpPatch("{id}/last-login")]
    public IActionResult UpdateLastLogin(string id)
    {
        var key = $"user:{id}";

        if (!_db.KeyExists(key))
            return NotFound(new { message = "User not found." });

        var now = DateTime.UtcNow.ToString("o"); // ISO 8601

        // Single-field HashSet — only lastLogin is written.
        // name and email are completely untouched in Redis.
        // Equivalent to: HSET user:1 lastLogin "2026-05-22T07:46:24Z"
        _db.HashSet(key, "lastLogin", now);

        return Ok(new HashExampleLastLoginResponse(id, now));
    }
}
