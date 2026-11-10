using Microsoft.Extensions.DependencyInjection;
using Playbook.Application.Todos;

namespace Playbook.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TodoService>();

        return services;
    }
}
