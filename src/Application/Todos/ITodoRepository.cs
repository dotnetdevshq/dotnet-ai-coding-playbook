using Playbook.Domain.Todos;

namespace Playbook.Application.Todos;

/// <summary>
/// Storage for todo items. The application layer owns this interface. Infrastructure implements it.
/// </summary>
public interface ITodoRepository
{
    Task AddAsync(TodoItem todo, CancellationToken cancellationToken);

    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
