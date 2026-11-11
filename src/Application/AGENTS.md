# AGENTS.md for the Application layer

These rules apply to code in `src/Application`, in addition to the root `AGENTS.md`. They cover use cases and the interfaces for everything outside the application.

## What lives here

- One folder per feature (for example `Todos/`) with its service, its DTOs, and the interfaces it needs from the outside world.
- `ApplicationServiceRegistration.cs`: registers the services of this layer.

## Dependencies

- This project references `Domain` only. It must not use `Infrastructure`, `Api`, EF Core, or ASP.NET Core. An architecture test (`ApplicationDoesNotUseInfrastructureApiOrFrameworks`) fails the build when this is broken.
- The only package is `Microsoft.Extensions.DependencyInjection.Abstractions`, for service registration. Do not add a package without stating why it is needed, as the root rules require.
- When a use case needs something from outside, such as storage, email, or another system, define an interface here and implement it in `Infrastructure`. See `ITodoRepository`. Why: the use case then depends on what it needs, not on how it is done, and it can be tested with a fake.
- Shape each interface around what the use cases need, with one method per need. Do not expose `IQueryable<T>`, a `DbSet<T>`, or a generic repository with methods nobody calls.

## Use cases

- A use case is one public method on a service: load what it needs, call the domain, save, and return a DTO. See `TodoService.CreateAsync`.
- Business rules live in the domain entity, not here. If you are writing an `if` about what is allowed, it probably belongs in `Domain`. This layer decides the order of the steps.
- Do not use HTTP concepts here: no status codes, no `IResult`, no `HttpContext`, no problem details. Why: the same use case must work from an endpoint, a background job, or a test.
- Do not read the system clock directly. Inject `TimeProvider` and pass the time to the domain, as `TodoService` does. Source: [TimeProvider](https://learn.microsoft.com/dotnet/standard/datetime/timeprovider-overview).
- Services are `sealed`, get their dependencies through the constructor, and hold no state between calls.

## What goes in and what comes out

- Never return a domain entity from a use case. Return a DTO, such as `TodoDto`. Why: the caller would otherwise be able to change the entity outside a use case, and every field of the entity would become part of the public contract. The architecture test `ApiDoesNotUseDomain` fails when an entity reaches the API.
- DTOs are `sealed record` types with no behavior besides mapping from an entity.
- Take simple values or a request record as input, not an entity and not an API request type.
- Return `null` (as `GetAsync` does) or a result value for an expected outcome such as "not found". Do not throw for outcomes the caller is expected to handle.

## Async

- Every method that reaches I/O through an interface is `async`, ends in `Async`, takes a `CancellationToken` as its last parameter, and passes it on. The token has no default value. Why: a default lets a caller forget it without the compiler noticing.
- Never call `.Result` or `.Wait()`.
- Do not start work that outlives the request with `Task.Run` or a discarded task.

## Registration

- Register every service of this layer in `AddApplication`. `Api` calls that method and registers nothing from this layer by itself.
- Use the scoped lifetime for services, because they depend on scoped storage.

## Tests

- Every use case has unit tests in `tests/UnitTests`, with hand-written fakes for the interfaces (see `FakeTodoRepository`) and a fixed clock (see `FixedTimeProvider`).
- Unit tests here do not use a database or HTTP. Tests of the real storage are in `tests/IntegrationTests`.

## Opinions

These are choices of this playbook, not facts. Adapt them to your team.

- One service class per feature with a method per use case. A handler class per use case (often with a mediator library) is a common alternative. It gives smaller classes at the cost of more files and, with a library, one more dependency.
- Hand-written fakes instead of a mocking library. They keep the tests readable and add no package. A mocking library pays off when interfaces are large.
- Mapping by hand in a static method on the DTO, with no mapping library. The compiler then catches a missing property.
