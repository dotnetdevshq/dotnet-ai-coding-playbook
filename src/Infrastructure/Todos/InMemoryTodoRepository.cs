using System.Collections.Concurrent;
using Playbook.Application.Todos;
using Playbook.Domain.Todos;

namespace Playbook.Infrastructure.Todos;

/// <summary>
/// Temporary storage that keeps items in memory. It is replaced by EF Core in the data access step.
/// </summary>
internal sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

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
