using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using learning_testing.DTOs;
using learning_testing.Models;
using Xunit;

namespace learning_testing.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for the Todos API endpoints.
/// Uses <see cref="CustomWebApplicationFactory"/> which spins up a real
/// PostgreSQL database in Docker via Testcontainers, so these tests
/// exercise the full stack (controller → service → repository → database).
///
/// ─── LEARNING NOTES ──────────────────────────────────────────────────
///
/// [Fact] attribute (xUnit):
///   Marks a method as a test method. xUnit discovers and runs all [Fact]
///   methods in the test class automatically. Unlike NUnit's [Test] or
///   MSTest's [TestMethod], [Fact] means "this test has no parameters."
///   For parameterized tests, use [Theory] + [InlineData] instead.
///
/// Naming convention: MethodName_Scenario_ExpectedBehavior
///   Examples:
///     Create_ShouldReturn201_WithValidRequest
///     GetById_ShouldReturn404_WhenNotExists
///
/// IClassFixture&lt;T&gt;:
///   Tells xUnit to create one instance of CustomWebApplicationFactory and
///   share it across all tests in this class. The factory starts a real
///   PostgreSQL container (via Testcontainers) before any test and stops
///   it after all tests finish.
///
/// WebApplicationFactory:
///   ASP.NET Core's built-in test host. CreateClient() returns an
///   HttpClient that sends requests to an in-memory server — no real
///   HTTP port needed.
///
/// Testcontainers:
///   Runs Docker containers programmatically. Here we use PostgreSqlContainer
///   to get a real PostgreSQL instance for realistic integration tests.
/// ──────────────────────────────────────────────────────────────────────
/// </summary>
public class TodosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Constructor receives the factory via IClassFixture.
    /// CreateClient() returns an HttpClient pre-configured to call the in-memory test server.
    /// </summary>
    public TodosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturn201_WithValidRequest()
    {
        var request = new CreateTodoRequest()
        {
            Title = "Integration Test Todo",
            Description = "Created in integration test",
            Priority = Priority.High,
            DueDate = DateTime.SpecifyKind(new DateTime(2026, 12, 31), DateTimeKind.Utc)
        };

        var response = await _client.PostAsJsonAsync("/api/todos", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Integration Test Todo");
        todo.IsCompleted.Should().BeFalse();
        todo.Priority.Should().Be("High");
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WithTodos()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "GetAll Test",
            Priority = Priority.Medium
        };
        await _client.PostAsJsonAsync("/api/todos", createRequest);

        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        todos.Should().NotBeNull();
        todos!.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturn200_WhenExists()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "GetById Test",
            Priority = Priority.Low
        };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var response = await _client.GetAsync($"/api/todos/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        todo!.Title.Should().Be("GetById Test");
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenNotExists()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/todos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturn200_WhenExists()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "Before Update",
            Priority = Priority.Low
        };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var updateRequest = new UpdateTodoRequest
        {
            Title = "After Update",
            IsCompleted = true,
            Priority = Priority.High
        };
        var response = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();
        updated!.Title.Should().Be("After Update");
        updated.IsCompleted.Should().BeTrue();
        updated.Priority.Should().Be("High");
    }

    [Fact]
    public async Task Update_ShouldReturn404_WhenNotExists()
    {
        var id = Guid.NewGuid();
        var updateRequest = new UpdateTodoRequest
        {
            Title = "Test",
            IsCompleted = false,
            Priority = Priority.Low
        };

        var response = await _client.PutAsJsonAsync($"/api/todos/{id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ToggleComplete_ShouldReturn200_AndToggleState()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "Toggle Test",
            Priority = Priority.Medium
        };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        created!.IsCompleted.Should().BeFalse();

        var response = await _client.PatchAsync($"/api/todos/{created.Id}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await response.Content.ReadFromJsonAsync<TodoResponse>();
        toggled!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Search_ShouldReturn200_WithMatchingTodos()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "UniqueSearchTerm123",
            Priority = Priority.Medium
        };
        await _client.PostAsJsonAsync("/api/todos", createRequest);

        var response = await _client.GetAsync("/api/todos/search?q=UniqueSearchTerm123");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        todos.Should().NotBeNull();
        todos!.Should().ContainSingle();
        todos[0].Title.Should().Be("UniqueSearchTerm123");
    }

    [Fact]
    public async Task Search_ShouldReturn400_WhenQueryEmpty()
    {
        var response = await _client.GetAsync("/api/todos/search?q=");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenExists()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "To Delete",
            Priority = Priority.Low
        };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var response = await _client.DeleteAsync($"/api/todos/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenNotExists()
    {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/todos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBulk_ShouldReturn201_WithMultipleTodos()
    {
        var requests = new[]
        {
            new CreateTodoRequest { Title = "Bulk 1", Priority = Priority.Low },
            new CreateTodoRequest { Title = "Bulk 2", Priority = Priority.High },
            new CreateTodoRequest { Title = "Bulk 3", Priority = Priority.Medium }
        };

        var response = await _client.PostAsJsonAsync("/api/todos/bulk", requests);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        todos.Should().NotBeNull();
        todos!.Length.Should().Be(3);
    }

    [Fact]
    public async Task DeleteBulk_ShouldReturn204_AndRemoveTodos()
    {
        var ids = new List<Guid>();
        for (int i = 0; i < 2; i++)
        {
            var createRequest = new CreateTodoRequest
            {
                Title = $"Bulk Delete {i}",
                Priority = Priority.Low
            };
            var createResponse = await _client.PostAsJsonAsync("/api/todos", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
            ids.Add(created!.Id);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/todos/bulk")
        {
            Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(ids),
                System.Text.Encoding.UTF8,
                "application/json")
        };
        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAll_ShouldFilterByIsCompleted()
    {
        var completedRequest = new CreateTodoRequest
        {
            Title = "Completed Todo",
            Priority = Priority.Medium
        };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", completedRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        await _client.PatchAsync($"/api/todos/{created!.Id}/complete", null);

        var response = await _client.GetAsync("/api/todos?isCompleted=true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        todos!.All(t => t.IsCompleted).Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByPriority()
    {
        var createRequest = new CreateTodoRequest
        {
            Title = "High Priority Todo",
            Priority = Priority.High
        };
        await _client.PostAsJsonAsync("/api/todos", createRequest);

        var response = await _client.GetAsync("/api/todos?priority=High");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        todos!.All(t => t.Priority == "High").Should().BeTrue();
    }
}
