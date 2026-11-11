# AGENTS.md for the Domain layer

These rules apply to code in `src/Domain`, in addition to the root `AGENTS.md`. They cover entities, value objects, and business rules.

## What lives here

- One folder per feature (for example `Todos/`) with its entities and value objects.
- Nothing else. No interfaces for storage, no DTOs, no services that need I/O.

## Dependencies

- This project references no other project and no NuGet package. Do not add one. Why: business rules that depend on nothing can be read, tested, and reused without a database, a web server, or a framework upgrade getting in the way. An architecture test (`DomainUsesNoOtherLayerAndNoFramework`) fails the build when this is broken.
- No EF Core attributes, no JSON attributes, no ASP.NET Core types. Mapping to tables lives in `Infrastructure`. Mapping to HTTP lives in `Api`.
- If a rule seems to need a database lookup or another service, it does not belong in an entity. Say so, and put the orchestration in the application layer.

## Entities

- An entity can never exist in an invalid state. Check every rule when the entity is created and when it changes, and throw when a rule is broken. See `TodoItem.Create`.
- Create entities through a static factory method and keep the constructor private. Why: the factory has a name that says what happens, and there is one place where the creation rules live.
- No public setters. Change state through methods that are named after what happens in the business, such as `Complete`, not `SetCompletedAt`. Why: a setter lets any caller skip the rules. An architecture test (`DomainTypesHaveNoPublicSetters`) enforces this. Init-only properties are allowed, because they cannot change after creation.
- Do not add a public parameterless constructor or a setter because a tool asks for one. EF Core and the serializers used in this solution do not need them.
- Put limits such as `MaxTitleLength` in a public constant on the entity. Other layers read the constant instead of copying the number.
- Mark classes `sealed` unless inheritance is part of the design.

## Time, identifiers, and randomness

- Never read the system clock here. No `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.UtcNow`, or `TimeProvider`. Take the time as a parameter, as `Create` and `Complete` do. Why: rules that depend on the current time cannot be tested reliably otherwise.
- Use `DateTimeOffset`, not `DateTime`, for points in time. Why: a `DateTime` does not say which time zone it is in.
- An entity creates its own identifier with `Guid.NewGuid()`. The database does not generate it. Why: the entity is complete before it is saved, so the application layer can return it or refer to it without a round trip.

## Errors

- Throw `ArgumentException` (or a subclass) for an invalid argument, and `InvalidOperationException` for an operation that the current state does not allow.
- Decide on purpose whether repeating an operation is an error or does nothing, and write the decision in the method's summary. `Complete` does nothing the second time.
- Do not return error codes, booleans, or `null` to signal a broken rule.

## Async and I/O

- No `async`, no `Task`, no `CancellationToken`, no logging, and no file, network, or database access in this project. Domain code is plain, synchronous, in-memory code.

## Tests

- Every rule has a unit test in `tests/UnitTests`: one for the valid case and one for each way the rule can be broken.
- Domain tests need no fakes or mocks. If one seems necessary, the code under test probably does not belong in the domain.

## Opinions

These are choices of this playbook, not facts. Adapt them to your team.

- Exceptions for broken rules instead of a result type. Exceptions keep the sample small. A result type makes expected failures explicit and is a good choice when a use case has many of them.
- Identifiers are plain `Guid` values. Strongly typed identifiers prevent mixing up the ids of different entities, at the cost of more mapping code.
- No domain events in the sample. Add them when one change must trigger work in another part of the system.
