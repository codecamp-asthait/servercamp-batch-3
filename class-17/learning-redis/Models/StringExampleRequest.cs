using System.Text.Json;

namespace learning_redis.Models;

// Body for POST /string-example
public record StringExampleRequest(
    string Key,           // Redis key, e.g. "product:42"
    JsonElement Value,    // Any JSON value — object, array, string, number
    int TtlSeconds        // How long until Redis auto-deletes this key
);
