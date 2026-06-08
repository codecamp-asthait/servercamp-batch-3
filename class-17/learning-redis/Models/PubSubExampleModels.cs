namespace learning_redis.Models;

// Body for POST /pubsub-example/notify
public record PubSubExampleNotifyRequest(string Message);

// Returned by POST /pubsub-example/notify
public record PubSubExampleNotifyResponse(string Channel, string Message);