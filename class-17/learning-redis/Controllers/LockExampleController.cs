using learning_redis.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace learning_redis.Controllers;

[ApiController]
[Route("lock-example")]
public class LockExampleController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string StockKey = "inventory:stock";
    private const string LockKey = "lock:inventory";

    // POST /lock-example/inventory/seed — set the initial stock count
    [HttpPost("inventory/seed")]
    public IActionResult Seed([FromBody] LockExampleSeedRequest request)
    {
        _db.StringSet(StockKey, request.Count);
        return Ok(new LockExampleInventoryResponse(request.Count));
    }

    // GET /lock-example/inventory — read the current stock count
    [HttpGet("inventory")]
    public IActionResult GetInventory()
    {
        var raw = _db.StringGet(StockKey);
        var stock = raw.HasValue ? (int)raw : 0;
        return Ok(new LockExampleInventoryResponse(stock));
    }

    // POST /lock-example/inventory/decrement — decrement stock under a distributed lock
    [HttpPost("inventory/decrement")]
    public IActionResult Decrement()
    {
        // A unique token per request — only this request can release the lock it acquires.
        var token = Guid.NewGuid().ToString();

        // LockTake uses SET NX PX — atomic acquire.
        // Returns false immediately if another request holds the lock.
        // 5-second expiry auto-releases the lock if this process crashes.
        bool acquired = _db.LockTake(LockKey, token, TimeSpan.FromSeconds(5));

        if (!acquired)
            return Conflict(new { message = "Could not acquire lock. Try again." });

        try
        {
            // Read-modify-write is now safe — no other request can enter this block
            var raw = _db.StringGet(StockKey);
            var stock = raw.HasValue ? (int)raw : 0;

            if (stock <= 0)
                return BadRequest(new { message = "Out of stock." });

            _db.StringSet(StockKey, stock - 1);

            return Ok(new LockExampleDecrementResponse(stock - 1, "Decremented successfully."));
        }
        finally
        {
            // Always release — even if an exception is thrown above.
            // LockRelease checks the token: only the owner can release.
            _db.LockRelease(LockKey, token);
        }
    }
}
