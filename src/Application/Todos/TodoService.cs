using Playbook.Domain.Todos;

namespace Playbook.Application.Todos;

/// <summary>
/// Use cases for todo items.
/// </summary>
public sealed class TodoService
{
    public const int MaxTitleLength = TodoItem.MaxTitleLength;

    private readonly ITodoRepository _repository;
    private readonly TimeProvider _timeProvider;

    public TodoService(ITodoRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<TodoDto> CreateAsync(string title, CancellationToken cancellationToken)
    {
        TodoItem todo = TodoItem.Create(title, _timeProvider.GetUtcNow());

        await _repository.AddAsync(todo, cancellationToken);

        return TodoDto.FromEntity(todo);
    }

    public async Task<TodoDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        TodoItem? todo = await _repository.GetByIdAsync(id, cancellationToken);

        return todo is null ? null : TodoDto.FromEntity(todo);
    }
}
