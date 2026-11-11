using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Playbook.Infrastructure.Persistence;

/// <summary>
/// Used only by the <c>dotnet ef</c> command-line tool, for example to add a migration.
/// It lets the tool create the context without starting the API. The application never calls it.
/// </summary>
internal sealed class PlaybookDbContextFactory : IDesignTimeDbContextFactory<PlaybookDbContext>
{
    public PlaybookDbContext CreateDbContext(string[] args)
    {
        // A placeholder file name. Adding a migration compares the model with the snapshot
        // and does not open this database.
        DbContextOptions<PlaybookDbContext> options = new DbContextOptionsBuilder<PlaybookDbContext>()
            .UseSqlite("Data Source=design-time.db")
            .Options;

        return new PlaybookDbContext(options);
    }
}
