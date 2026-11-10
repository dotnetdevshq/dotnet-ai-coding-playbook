using Microsoft.AspNetCore.Http.HttpResults;
using Playbook.Application.Todos;

namespace Playbook.Api.Endpoints;

/// <summary>
/// HTTP endpoints for todo items. Endpoints stay thin: validate input, call the application layer, map the result.
/// </summary>
internal static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/todos");

        group.MapPost("/", CreateAsync);
        group.MapGet("/{id:guid}", GetAsync);

        return app;
    }

    private static async Task<Results<Created<TodoDto>, ValidationProblem>> CreateAsync(
        CreateTodoRequest request,
        TodoService service,
        CancellationToken cancellationToken)
    {
        string title = request.Title?.Trim() ?? string.Empty;

        if (title.Length == 0 || title.Length > TodoService.MaxTitleLength)
        {
            Dictionary<string, string[]> errors = new()
            {
                ["title"] = [$"A title is required and can have at most {TodoService.MaxTitleLength} characters."],
            };

            return TypedResults.ValidationProblem(errors);
        }

        TodoDto todo = await service.CreateAsync(title, cancellationToken);

        return TypedResults.Created($"/todos/{todo.Id}", todo);
    }

    private static async Task<Results<Ok<TodoDto>, NotFound>> GetAsync(
        Guid id,
        TodoService service,
        CancellationToken cancellationToken)
    {
        TodoDto? todo = await service.GetAsync(id, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(todo);
    }
}
