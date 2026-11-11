using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Playbook.IntegrationTests;

/// <summary>
/// Runs the real API in memory against its own temporary SQLite database.
/// The API applies the migrations at startup because the environment is Development.
/// </summary>
public sealed class PlaybookApiFactory : WebApplicationFactory<Program>
{
    private readonly TemporaryDatabase _database = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Playbook", _database.ConnectionString);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _database.Dispose();
        }
    }
}
