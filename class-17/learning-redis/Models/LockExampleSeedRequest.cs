namespace learning_redis.Models;

// Body for POST /lock-example/inventory/seed
public record LockExampleSeedRequest(int Count);

// Returned by GET /lock-example/inventory and POST /lock-example/inventory/seed
public record LockExampleInventoryResponse(int Inventory);

// Returned by POST /lock-example/inventory/decrement
public record LockExampleDecrementResponse(int Inventory, string Message);