namespace Playbook.IntegrationTests;

/// <summary>
/// A SQLite database file that exists for the lifetime of one test class.
/// Tests run against a real database, so they also exercise the migrations and the SQL that EF Core generates.
/// </summary>
internal sealed class TemporaryDatabase : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"playbook-tests-{Guid.NewGuid():N}.db");

    // Pooling is off so that no connection keeps the file open after a test has finished.
    public string ConnectionString => $"Data Source={_path};Pooling=False";

    public void Dispose()
    {
        File.Delete(_path);
    }
}
