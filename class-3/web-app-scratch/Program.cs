// create a router
var router = new Router();

// register the router
router.MapGet("/user", (ctx) => $"Fetching user from route: {ctx.Path}");
router.MapPost("/user", (ctx) => $"Creating user at route: {ctx.Path}");

// create server
var server = new TcpServer(5005, router);

// run the server
await server.StartAsync();
