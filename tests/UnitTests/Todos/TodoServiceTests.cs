using Playbook.Application.Todos;

namespace Playbook.UnitTests.Todos;

public sealed class TodoServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 15, 9, 0, 0, TimeSpan.Zero);

    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _service = new TodoService(_repository, new FixedTimeProvider(Now));
    }

    [Fact]
    public async Task CreateStoresTheItemAndUsesTheClock()
    {
        TodoDto created = await _service.CreateAsync("Write the playbook", CancellationToken.None);

        Assert.Equal("Write the playbook", created.Title);
        Assert.Equal(Now, created.CreatedAt);
        Assert.False(created.IsCompleted);
        Assert.Equal(1, _repository.Count);
    }

    [Fact]
    public async Task GetReturnsAnItemThatWasCreated()
    {
        TodoDto created = await _service.CreateAsync("Write the playbook", CancellationToken.None);

        TodoDto? found = await _service.GetAsync(created.Id, CancellationToken.None);

        Assert.Equal(created, found);
    }

    [Fact]
    public async Task GetReturnsNullForAnUnknownId()
    {
        TodoDto? found = await _service.GetAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(found);
    }
}
