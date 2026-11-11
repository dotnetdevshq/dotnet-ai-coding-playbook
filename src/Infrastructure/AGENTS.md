# AGENTS.md for the Infrastructure layer

These rules apply to code in `src/Infrastructure`, in addition to the root `AGENTS.md`. They cover data access with EF Core.

## What lives here

- `Persistence/`: the `DbContext`, one `IEntityTypeConfiguration` class per entity, and the migrations.
- One folder per feature (for example `Todos/`) with the implementations of the interfaces that the application layer defines.
- `InfrastructureServiceRegistration.cs`: the only public type. Everything else is `internal`.

The sample uses SQLite so that it runs without a database server. The rules below apply to any relational provider unless they say otherwise.

## Boundaries

- Keep the `DbContext`, the configurations, and the repositories `internal`. Why: other layers then cannot query the database directly, even by accident. The compiler enforces this, and an architecture test checks that `Api` does not use EF Core.
- Do not return `IQueryable<T>` or `DbSet<T>` from a repository. Return entities, DTOs, or lists. Why: a leaked `IQueryable` lets callers build queries outside this layer, where nobody reviews the SQL.
- Map entities in `IEntityTypeConfiguration` classes, not with attributes on the entity. Why: the domain layer must not depend on EF Core.
- Do not add public setters or a public parameterless constructor to an entity to make EF Core happy. EF Core can use private constructors and read-only properties when the configuration names them. See `TodoItemConfiguration.cs`.

## Queries

- Use `AsNoTracking()` on every query whose result is only read. Why: change tracking costs memory and time, and it is only needed when the entity will be saved again. Source: [Tracking vs. no-tracking queries](https://learn.microsoft.com/ef/core/querying/tracking).
- Load an entity with a tracked query when the use case changes it and saves it.
- Filter, sort, and page in the database. Do not call `ToListAsync()` and then filter in memory. Why: that loads the whole table into the application.
- Select only what the caller needs. For read models, project to a DTO with `Select` instead of loading whole entities. Source: [Efficient querying](https://learn.microsoft.com/ef/core/performance/efficient-querying).
- Never run a query inside a loop over the results of another query (the N+1 problem). Load related data in the same query with `Include` or a projection.
- When a query includes more than one collection, decide between a single query and `AsSplitQuery()` on purpose, and say why in a comment.
- Give every list query a deterministic `OrderBy` and a limit. Why: without a limit, the query grows with the table.

## Async

- Use the async methods (`ToListAsync`, `SingleOrDefaultAsync`, `SaveChangesAsync`) and pass the `CancellationToken` to each of them.
- Never call `.Result` or `.Wait()` on a database call.
- Do not run two operations on the same `DbContext` at the same time, for example with `Task.WhenAll`. Why: a `DbContext` is not thread-safe.

## Raw SQL

- Prefer LINQ. Use raw SQL only when LINQ cannot express the query, and say why in a comment.
- Never build SQL by concatenating or interpolating user input into `FromSqlRaw` or `ExecuteSqlRaw`. Use `FromSql` or `ExecuteSql` with an interpolated string, which turns the values into parameters. Source: [SQL queries](https://learn.microsoft.com/ef/core/querying/sql-queries).

## Migrations

- Every change to an entity or its configuration needs a migration in the same pull request. The test `MigrationsMatchTheModel` fails when one is missing.
- Add a migration with the local tool, from the repository root:

  ```bash
  dotnet tool restore
  dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Infrastructure --framework net10.0 --output-dir Persistence/Migrations
  dotnet format
  ```

  `dotnet format` is needed because the generated code does not follow the style rules of this repository, and warnings are errors.
- Read the generated migration before committing it. Why: a rename is often generated as a drop and an add, which deletes data.
- Never edit or delete a migration that has been merged into `main`. Add a new migration instead. Why: databases that already applied the old one would no longer match the code.
- Do not write the model snapshot by hand. The tool owns it.
- The API applies migrations at startup in the Development environment only. In production, apply them as a separate deployment step. Source: [Applying migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying).
- Both target frameworks share one set of migrations. Generate them with `--framework net10.0`, the lower EF Core version, so they compile and run on both. The integration tests run the migrations on both frameworks.

## Connection strings

- Read the connection string from configuration. `Api` passes it to `AddInfrastructure`.
- The connection string in `appsettings.json` is a local SQLite file name and contains no secret. Never commit a connection string with a password. Use user secrets or environment variables for real values.

## SQLite specifics

- SQLite cannot compare or sort `DateTimeOffset`, `decimal`, or `TimeSpan` values in SQL. A query that tries throws at run time. Source: [SQLite provider limitations](https://learn.microsoft.com/ef/core/providers/sqlite/limitations). If you need such a query, say so instead of sorting in memory.
- SQLite does not enforce `HasMaxLength`. Length rules are enforced by the domain entity.

## Tests

- Test data access against a real database, not the EF Core in-memory provider. Why: the in-memory provider does not run SQL, so it hides translation errors and constraint violations. The integration tests create a temporary SQLite file and apply the migrations to it.
- Save with one `DbContext` and read with another, so the test proves the data reached the database.

## Opinions

These are choices of this playbook, not facts about EF Core. Adapt them to your team.

- A repository per feature in front of the `DbContext`. Some teams use the `DbContext` directly in the application layer. That is a valid trade, but it lets EF Core into `Application`, which this sample forbids.
- Repositories call `SaveChangesAsync` themselves. This is simple for single-entity use cases. A use case that changes several aggregates in one transaction needs a unit of work instead.
