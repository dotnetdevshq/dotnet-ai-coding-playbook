using Microsoft.EntityFrameworkCore;
using Playbook.Application.Todos;
using Playbook.Domain.Todos;
using Playbook.Infrastructure.Persistence;

namespace Playbook.Infrastructure.Todos;

/// <summary>
/// Stores todo items in the database through EF Core.
/// </summary>
internal sealed class EfTodoRepository : ITodoRepository
{
    private readonly PlaybookDbContext _context;

    public EfTodoRepository(PlaybookDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TodoItem todo, CancellationToken cancellationToken)
    {
        _context.Todos.Add(todo);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Every current caller only reads the item, so change tracking is switched off.
        // A use case that changes an item must load it with a tracked query instead.
        return await _context.Todos
            .AsNoTracking()
            .SingleOrDefaultAsync(todo => todo.Id == id, cancellationToken);
    }
}
