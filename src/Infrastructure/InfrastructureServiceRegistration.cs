using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Playbook.Application.Todos;
using Playbook.Infrastructure.Todos;

namespace Playbook.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

        return services;
    }
}
