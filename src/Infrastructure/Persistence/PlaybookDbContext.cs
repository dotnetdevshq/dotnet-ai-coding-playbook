using Microsoft.EntityFrameworkCore;
using Playbook.Domain.Todos;

namespace Playbook.Infrastructure.Persistence;

/// <summary>
/// The EF Core unit of work. It is internal, so no other layer can use it directly.
/// Other layers reach storage through the interfaces that the application layer defines.
/// </summary>
internal sealed class PlaybookDbContext : DbContext
{
    public PlaybookDbContext(DbContextOptions<PlaybookDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapping lives in one IEntityTypeConfiguration class per entity, next to this file.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlaybookDbContext).Assembly);
    }
}
