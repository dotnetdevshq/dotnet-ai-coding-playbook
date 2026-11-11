using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Playbook.Application.Todos;
using Playbook.Infrastructure.Persistence;
using Playbook.Infrastructure.Todos;

namespace Playbook.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<PlaybookDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ITodoRepository, EfTodoRepository>();

        return services;
    }

    /// <summary>
    /// Creates the database if it is missing and applies every migration that has not run yet.
    /// The composition root decides when to call this. The sample calls it in development only.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using AsyncServiceScope scope = services.CreateAsyncScope();
        PlaybookDbContext context = scope.ServiceProvider.GetRequiredService<PlaybookDbContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
