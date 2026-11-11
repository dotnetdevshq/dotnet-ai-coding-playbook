using Microsoft.EntityFrameworkCore;
using Playbook.Domain.Todos;
using Playbook.Infrastructure.Persistence;
using Playbook.Infrastructure.Todos;

namespace Playbook.IntegrationTests;

/// <summary>
/// Exercises the repository against a real SQLite database that is created from the migrations.
/// Each test saves with one context and reads with another, so it proves the data reached the database.
/// </summary>
public sealed class EfTodoRepositoryTests : IDisposable
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 11, 12, 9, 0, 0, TimeSpan.Zero);

    private readonly TemporaryDatabase _database = new();

    public EfTodoRepositoryTests()
    {
        using PlaybookDbContext context = CreateContext();
        context.Database.Migrate();
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    [Fact]
    public async Task AddedTodoCanBeReadBackWithEveryValue()
    {
        TodoItem todo = TodoItem.Create("Write the playbook", CreatedAt);

        await using (PlaybookDbContext context = CreateContext())
        {
            await new EfTodoRepository(context).AddAsync(todo, CancellationToken.None);
        }

        await using PlaybookDbContext readContext = CreateContext();
        TodoItem? stored = await new EfTodoRepository(readContext).GetByIdAsync(todo.Id, CancellationToken.None);

        Assert.NotNull(stored);
        Assert.Equal(todo.Id, stored.Id);
        Assert.Equal("Write the playbook", stored.Title);
        Assert.Equal(CreatedAt, stored.CreatedAt);
        Assert.Null(stored.CompletedAt);
        Assert.False(stored.IsCompleted);
    }

    [Fact]
    public async Task CompletionTimeIsStored()
    {
        DateTimeOffset completedAt = CreatedAt.AddHours(2);
        TodoItem todo = TodoItem.Create("Review the pull request", CreatedAt);
        todo.Complete(completedAt);

        await using (PlaybookDbContext context = CreateContext())
        {
            await new EfTodoRepository(context).AddAsync(todo, CancellationToken.None);
        }

        await using PlaybookDbContext readContext = CreateContext();
        TodoItem? stored = await new EfTodoRepository(readContext).GetByIdAsync(todo.Id, CancellationToken.None);

        Assert.NotNull(stored);
        Assert.Equal(completedAt, stored.CompletedAt);
        Assert.True(stored.IsCompleted);
    }

    [Fact]
    public async Task GetByIdReturnsNullForAnUnknownId()
    {
        await using PlaybookDbContext context = CreateContext();

        TodoItem? stored = await new EfTodoRepository(context).GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(stored);
    }

    [Fact]
    public async Task GetByIdDoesNotTrackTheEntity()
    {
        TodoItem todo = TodoItem.Create("Read without tracking", CreatedAt);

        await using (PlaybookDbContext context = CreateContext())
        {
            await new EfTodoRepository(context).AddAsync(todo, CancellationToken.None);
        }

        await using PlaybookDbContext readContext = CreateContext();
        await new EfTodoRepository(readContext).GetByIdAsync(todo.Id, CancellationToken.None);

        Assert.Empty(readContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task MigrationsMatchTheModel()
    {
        // Fails when someone changes an entity or its mapping without adding a migration.
        await using PlaybookDbContext context = CreateContext();

        Assert.False(context.Database.HasPendingModelChanges());
    }

    private PlaybookDbContext CreateContext()
    {
        DbContextOptions<PlaybookDbContext> options = new DbContextOptionsBuilder<PlaybookDbContext>()
            .UseSqlite(_database.ConnectionString)
            .Options;

        return new PlaybookDbContext(options);
    }
}
