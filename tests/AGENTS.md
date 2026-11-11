# AGENTS.md for the tests

These rules apply to code in `tests/`, in addition to the root `AGENTS.md`. They cover what to test, where, and how.

## What lives here

| Project | What it tests | What it may use |
|---|---|---|
| `UnitTests` | Domain rules and application use cases | Plain objects, hand-written fakes, a fixed clock. No database, no HTTP, no file system. |
| `IntegrationTests` | The API over HTTP and the data access code | The real API in memory and a real temporary SQLite database |
| `ArchitectureTests` | The dependency and design rules from the `AGENTS.md` files | Reflection over the compiled assemblies |

- `Directory.Build.props` in this folder adds the test packages to every test project. Do not add them to a project file.
- `Shared/TestConventionTests.cs` is compiled into every test project and checks the tests of that project.

## Never weaken a test

- Never skip, delete, comment out, or loosen a test to get a green build. If a test fails, the code or the test is wrong. Find out which, and fix that. Why: a weakened test still looks like coverage, so nobody notices the gap.
- The test `NoTestIsSkipped` fails when a test or a data row has `Skip` set. It cannot see a test that was deleted or commented out. Reviewers must look for those in the diff.
- If you believe a test is wrong, say why in the pull request and change the test in a way that still checks the behavior.

## What to test

- Every change in behavior comes with a test that fails without the change.
- Test behavior through the public surface: the methods of an entity, the methods of a service, the HTTP endpoints. Do not test private methods, and do not assert on how the code reaches its result. Why: tests tied to implementation details break on every refactoring and stop people from improving the code.
- Cover the valid case and each way a rule can be broken. Cover the boundary, as `CreateAcceptsATitleOfExactlyTheMaximumLength` does.
- Put the test at the lowest level that can prove the behavior. A domain rule is a unit test. A status code or a database mapping is an integration test.
- A rule that can be checked by reflection belongs in `ArchitectureTests`, so the build enforces it instead of a reviewer.

## How to write a test

- Name a test as a sentence about the behavior, in PascalCase, without underscores: `GetReturnsNullForAnUnknownId`. The name must make sense in a failure report without opening the file.
- One behavior per test. Several assertions are fine when they describe the same outcome.
- Structure a test as arrange, act, assert, separated by blank lines. No `if`, loops, or `try`/`catch` in a test.
- Use `[Theory]` with `[InlineData]` when the same behavior is checked with different inputs.
- Test classes are `public sealed`. Helpers and fakes are `internal sealed`.
- Use literal expected values. Do not compute the expected value with the same logic as the code under test.

## Determinism

- A test gives the same result every time, on every machine, in any order.
- Never read the real clock. Use `FixedTimeProvider` and fixed `DateTimeOffset` values.
- No `Thread.Sleep`, no `Task.Delay`, and no retries to make a test pass. A test that needs to wait is waiting for something specific. Await that instead.
- No shared mutable state between tests. Each test creates what it needs. xUnit creates a new instance of the test class for every test, so instance fields are safe and static fields are not.
- Tests do not call real external services and do not need network access.

## Fakes

- Replace an interface of the application layer with a hand-written fake, such as `FakeTodoRepository`. Keep the fake as simple as an in-memory collection.
- Fake only what you own: the interfaces defined in `Application`. Do not fake `DbContext`, `HttpClient`, or framework types.
- Do not add a mocking library without the justification that the root rules require for any new package.

## Integration tests

- Use `PlaybookApiFactory` to run the API. It gives each test class its own temporary database and applies the migrations.
- Do not use the EF Core in-memory provider. Why: it does not run SQL, so it hides translation errors and constraint violations.
- Declare the response types a test reads inside the test class. Do not reuse the DTOs of the application layer. Why: a test that reuses the DTO still passes when a property is renamed, although the change breaks every client.
- Assert on the status code first, then on the body.
- A test cleans up what it creates outside the process, as `TemporaryDatabase` deletes its file.

## Architecture tests

- When you add a rule to an `AGENTS.md` that can be checked automatically, add the check in the same pull request.
- Prove that a new check can fail: break the rule on purpose, see the test fail, and undo the change. Say in the pull request that you did. Why: a check that can never fail protects nothing.
- Failure messages name the offending type or member, so the fix is obvious.

## Running the tests

- `dotnet test --no-build` from the repository root runs every test on both target frameworks. All of them must pass before a change is finished.

## Opinions

These are choices of this playbook, not facts. Adapt them to your team.

- xUnit as the test framework. NUnit, MSTest, and TUnit work as well. The rules above do not depend on the framework, except for the `Skip` check.
- The assertions that come with xUnit, with no assertion library.
- No code coverage threshold. A threshold is easy to satisfy with tests that assert nothing. The rule "every change in behavior has a test that fails without it" is checked in review instead.
- Test names without underscores. `Method_State_Result` is a common alternative.
