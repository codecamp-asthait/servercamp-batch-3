namespace learning_redis.Models;

// Body for POST /sorted-set-example/view
public record SortedSetExampleViewRequest(string ProductId);

// Returned by POST /sorted-set-example/view
public record SortedSetExampleViewResponse(string ProductId, long Views);

// Single entry in the leaderboard
public record SortedSetExampleLeaderboardEntry(string ProductId, long Views);

// Returned by GET /sorted-set-example/top
public record SortedSetExampleTopResponse(SortedSetExampleLeaderboardEntry[] Leaderboard);
