using learning_testing.DTOs;
using learning_testing.Models;
using learning_testing.Repositories;
using learning_testing.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace learning_testing.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="TodoService"/>.
/// Uses Moq to mock <see cref="ITodoRepository"/> so the tests focus purely
/// on the service layer's business logic and DTO mapping — no database needed.
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
///     CreateAsync_ShouldCreateTodo_WithCorrectProperties
///     GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNotExists
///   This makes test output readable: you know exactly what's being tested,
///   under what condition, and what should happen.
///
/// Arrange-Act-Assert (AAA):
///   Each test follows three sections separated by blank lines:
///     Arrange   - set up objects, mocks, and inputs
///     Act       - invoke the method under test
///     Assert    - verify the result using FluentAssertions
///
/// System Under Test (_sut):
///   The instance being tested. Naming it _sut (instead of _service) keeps
///   the pattern consistent across all test classes.
///
/// Moq:
///   Mock&lt;ITodoRepository&gt; creates a fake repository. Setup() configures
///   what it returns; Verify() checks it was called correctly.
/// ──────────────────────────────────────────────────────────────────────
/// </summary>
public class TodoServiceTests
{
    /// <summary>Mock repository — records calls and returns configured data.</summary>
    private readonly Mock<ITodoRepository> _mockRepository;

    /// <summary>System Under Test — the TodoService instance being tested.</summary>
    private readonly TodoService _sut;

    /// <summary>
    /// Setup runs before every test. Creates a fresh mock and SUT so tests are isolated.
    /// </summary>
    public TodoServiceTests()
    {
        _mockRepository = new Mock<ITodoRepository>();
        _sut = new TodoService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTodo_WithCorrectProperties()
    {
        var request = new CreateTodoRequest
        {
            Title = "Test Todo",
            Description = "Test Description",
            Priority = Priority.High,
            DueDate = new DateTime(2026, 12, 31)
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        var result = await _sut.CreateAsync(request);

        result.Title.Should().Be("Test Todo");
        result.Description.Should().Be("Test Description");
        result.Priority.Should().Be("High");
        result.IsCompleted.Should().BeFalse();
        result.DueDate.Should().Be(new DateTime(2026, 12, 31));

        // Verify the repository received a Todo with expected system-set fields.
        _mockRepository.Verify(r => r.CreateAsync(It.Is<Todo>(t =>
            t.Title == "Test Todo" &&
            t.IsCompleted == false &&
            t.CreatedAt != default &&
            t.UpdatedAt != default
        )), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodo_WhenExists()
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = "Existing Todo",
            IsCompleted = false,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(todo.Id))
            .ReturnsAsync(todo);

        var result = await _sut.GetByIdAsync(todo.Id);

        result.Id.Should().Be(todo.Id);
        result.Title.Should().Be("Existing Todo");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        var id = Guid.NewGuid();
        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Todo?)null);

        var act = () => _sut.GetByIdAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodo_WhenExists()
    {
        var id = Guid.NewGuid();
        var existing = new Todo
        {
            Id = id,
            Title = "Old Title",
            IsCompleted = false,
            Priority = Priority.Low,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existing);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        var request = new UpdateTodoRequest
        {
            Title = "New Title",
            Description = "Updated",
            IsCompleted = true,
            Priority = Priority.High,
            DueDate = null
        };

        var result = await _sut.UpdateAsync(id, request);

        result.Title.Should().Be("New Title");
        result.IsCompleted.Should().BeTrue();
        result.Priority.Should().Be("High");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        var id = Guid.NewGuid();
        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Todo?)null);

        var request = new UpdateTodoRequest
        {
            Title = "Test",
            IsCompleted = false,
            Priority = Priority.Low
        };

        var act = () => _sut.UpdateAsync(id, request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ToggleCompleteAsync_ShouldToggleFromFalseToTrue()
    {
        var id = Guid.NewGuid();
        var todo = new Todo
        {
            Id = id,
            Title = "Test",
            IsCompleted = false,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(todo);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        var result = await _sut.ToggleCompleteAsync(id);

        result.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleCompleteAsync_ShouldToggleFromTrueToFalse()
    {
        var id = Guid.NewGuid();
        var todo = new Todo
        {
            Id = id,
            Title = "Test",
            IsCompleted = true,
            Priority = Priority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(todo);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        var result = await _sut.ToggleCompleteAsync(id);

        result.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePriorityAsync_ShouldUpdatePriority()
    {
        var id = Guid.NewGuid();
        var todo = new Todo
        {
            Id = id,
            Title = "Test",
            IsCompleted = false,
            Priority = Priority.Low,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(todo);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        var result = await _sut.UpdatePriorityAsync(id, Priority.High);

        result.Priority.Should().Be("High");
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository_WhenExists()
    {
        var id = Guid.NewGuid();
        var todo = new Todo { Id = id, Title = "Test" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(todo);

        await _sut.DeleteAsync(id);

        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        var id = Guid.NewGuid();
        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Todo?)null);

        var act = () => _sut.DeleteAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedResponses()
    {
        var filter = new TodoFilter { IsCompleted = false };
        var todos = new List<Todo>
        {
            new Todo
            {
                Id = Guid.NewGuid(),
                Title = "Todo 1",
                IsCompleted = false,
                Priority = Priority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Todo
            {
                Id = Guid.NewGuid(),
                Title = "Todo 2",
                IsCompleted = false,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync(todos);

        var result = await _sut.GetAllAsync(filter);

        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Todo 1");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMappedResponses()
    {
        var todos = new List<Todo>
        {
            new Todo
            {
                Id = Guid.NewGuid(),
                Title = "Search Result",
                IsCompleted = false,
                Priority = Priority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockRepository
            .Setup(r => r.SearchAsync("Search"))
            .ReturnsAsync(todos);

        var result = await _sut.SearchAsync("Search");

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Search Result");
    }

    [Fact]
    public async Task CreateBulkAsync_ShouldCreateMultipleTodos()
    {
        var requests = new List<CreateTodoRequest>
        {
            new CreateTodoRequest { Title = "Bulk 1", Priority = Priority.Low },
            new CreateTodoRequest { Title = "Bulk 2", Priority = Priority.High }
        };

        _mockRepository
            .Setup(r => r.CreateBulkAsync(It.IsAny<IEnumerable<Todo>>()))
            .ReturnsAsync((IEnumerable<Todo> t) => t);

        var result = await _sut.CreateBulkAsync(requests);

        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.CreateBulkAsync(It.Is<IEnumerable<Todo>>(
            todos => todos.Count() == 2
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteBulkAsync_ShouldCallRepositoryWithIds()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        await _sut.DeleteBulkAsync(ids);

        _mockRepository.Verify(r => r.DeleteBulkAsync(ids), Times.Once);
    }
}
