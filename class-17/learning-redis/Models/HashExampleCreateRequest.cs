namespace learning_redis.Models;

// Body for POST /hash-example
public record HashExampleCreateRequest(string Id, string Name, string Email);

// Returned by GET /hash-example/{id} and POST /hash-example
public record HashExampleProfileResponse(string Id, string Name, string Email, string? LastLogin);

// Returned by PATCH /hash-example/{id}/last-login
public record HashExampleLastLoginResponse(string Id, string LastLogin);
