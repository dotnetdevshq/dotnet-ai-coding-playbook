using Playbook.Application.Todos;
using Playbook.Domain.Todos;

namespace Playbook.UnitTests.Todos;

internal sealed class FakeTodoRepository : ITodoRepository
{
    private readonly Dictionary<Guid, TodoItem> _items = [];

    public int Count => _items.Count;

    public Task AddAsync(TodoItem todo, CancellationToken cancellationToken)
    {
        _items[todo.Id] = todo;

        return Task.CompletedTask;
    }

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _items.TryGetValue(id, out TodoItem? todo);

        return Task.FromResult(todo);
    }
}
