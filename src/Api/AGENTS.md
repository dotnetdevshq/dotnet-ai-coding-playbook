# AGENTS.md for the API layer

These rules apply to code in `src/Api`, in addition to the root `AGENTS.md`. They cover HTTP endpoints and the composition root.

## What lives here

- `Program.cs`: the composition root. It reads configuration, registers the services of the other layers, and maps the endpoints.
- `Endpoints/`: one static class per feature (for example `TodoEndpoints`) with a `Map...Endpoints` extension method, and the request types for that feature.

The sample uses minimal APIs. The rules below apply to controllers too, unless they say otherwise.

## Boundaries

- An endpoint does three things: validate the input, call the application layer, and map the result to an HTTP response. Why: business rules in an endpoint cannot be reused or unit tested without HTTP.
- Do not use EF Core, a `DbContext`, or a repository in this project. Call an application service instead. An architecture test (`ApiDoesNotUseEntityFramework`) fails the build when EF Core is used here.
- Do not use domain entities in this project. Accept request types defined here and return the DTOs that the application layer returns. Why: returning an entity exposes every field it has, now and in the future, and ties the public contract to the domain model. An architecture test (`ApiDoesNotUseDomain`) enforces this.
- `Infrastructure` is referenced for one purpose: calling `AddInfrastructure` and `ApplyMigrationsAsync` in `Program.cs`. Do not use anything else from it.
- Keep request types and endpoint classes `internal`. Nothing outside this project needs them.

## Endpoints

- Group the endpoints of a feature with `MapGroup` and register them from one `Map...Endpoints` method. `Program.cs` only calls that method.
- Write handlers as named `private static` methods, not inline lambdas. Why: named methods keep `MapPost` and `MapGet` lines readable and show up by name in stack traces.
- Return `TypedResults`, and declare every possible response in the return type, for example `Task<Results<Ok<TodoDto>, NotFound>>`. Why: the compiler then rejects a response that the signature does not declare, and the response types are visible to API description tools. Source: [Create responses in minimal API applications](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/responses).
- Make every handler that does I/O `async`, accept a `CancellationToken`, and pass it to the application layer. Why: when the client disconnects, the request stops instead of finishing work nobody will read.
- Never call `.Result` or `.Wait()`.
- Do not catch exceptions in a handler to turn them into responses. Expected outcomes, such as "not found", are return values of the application layer. Unexpected errors go to the exception handler.

## Status codes

- `201 Created` with a `Location` header for a successful create.
- `200 OK` for a successful read.
- `400 Bad Request` with validation problem details when the input is invalid.
- `404 Not Found` when the resource does not exist.
- Do not return `200 OK` with an error message in the body.

## Validation

- Validate everything that comes from the request before calling the application layer. Request properties are nullable (`string?`) because a client can omit any of them.
- Return validation errors with `TypedResults.ValidationProblem`, keyed by the name of the field.
- Do not copy limits into this project. Read them from the application layer, as `TodoService.MaxTitleLength` does. Why: a copied number drifts when the rule changes.
- Validation here is for a clear error message. It does not replace the rules in the domain entity, which stay in place for every caller.

## Errors

- Errors use problem details ([RFC 9457](https://www.rfc-editor.org/rfc/rfc9457)). `AddProblemDetails` and `UseExceptionHandler` are registered in `Program.cs`. Do not remove them.
- Never put a stack trace, an exception message, a SQL statement, or a connection string in a response. See `SECURITY.md`.

## Authentication and authorization

- The sample endpoints are anonymous on purpose, because the sample has no users. This is a decision for the sample, not a default to copy.
- When you add an endpoint, state in the pull request who may call it. If the project has authentication, require it on the route group and opt out per endpoint, not the other way round. Why: a forgotten opt-in leaves an endpoint open, while a forgotten opt-out only fails closed.
- Never trust an identifier from the request to decide what a caller may see. Check that the caller has access to that record.

## Configuration

- Read configuration in `Program.cs` and pass values to the other layers. Fail at startup with a clear message when a required value is missing, as the connection string check does.
- `appsettings.json` holds no secrets. Use user secrets or environment variables for real values.
- Migrations are applied at startup in the Development environment only. Do not widen that condition.

## Tests

- Every endpoint has integration tests in `tests/IntegrationTests` that call it over HTTP through `PlaybookApiFactory`. Cover the success response and each error response that the handler declares.
- Tests declare their own response types instead of reusing the DTOs from the application layer. Why: a test that reuses the DTO still passes when a property is renamed, although the change breaks every client.

## Opinions

These are choices of this playbook, not facts about ASP.NET Core. Adapt them to your team.

- Minimal APIs instead of controllers. Both work. Minimal APIs keep the sample small.
- Validation by hand in the handler. It is explicit and has no dependencies. With many endpoints, a validation library or the built-in validation support is less repetitive.
- No API versioning and no OpenAPI document in the sample yet. Add them before the first external client depends on the API.
