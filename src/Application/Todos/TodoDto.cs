using Playbook.Domain.Todos;

namespace Playbook.Application.Todos;

/// <summary>
/// What callers outside the application layer see. Entities are never returned directly.
/// </summary>
public sealed record TodoDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt)
{
    public static TodoDto FromEntity(TodoItem todo)
    {
        return new TodoDto(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt, todo.CompletedAt);
    }
}
