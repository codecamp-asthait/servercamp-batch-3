namespace learning_redis.Models;

// Body for POST /set-example/visit
public record SetExampleVisitRequest(string IpAddress);

// Returned by POST /set-example/visit
public record SetExampleVisitResponse(string IpAddress, bool IsNewVisitor);

// Returned by GET /set-example/unique-count
public record SetExampleUniqueCountResponse(string Date, long UniqueVisitors);