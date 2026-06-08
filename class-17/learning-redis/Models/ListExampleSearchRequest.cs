namespace learning_redis.Models;

// Body for POST /list-example/search
public record ListExampleSearchRequest(string UserId, string Term);

// Returned by POST /list-example/search
public record ListExampleSearchResponse(string UserId, string Term);

// Returned by GET /list-example/search/history
public record ListExampleHistoryResponse(string UserId, string[] Searches);
