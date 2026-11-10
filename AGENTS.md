# AGENTS.md

Instructions for AI coding agents working in this repository. Human contributors should follow them too.

## What this repository is

A public playbook and template that helps .NET developers keep AI-written code up to their standards. It combines rules that agents read (files like this one) with guardrails the build enforces (analyzers, architecture tests, CI).

Everything here is meant to be copied into other people's projects. Prefer simple, conventional, well-explained solutions over clever ones.

## Current status

The repository is in its setup phase. The sample solution described below does not exist yet, so the build and test commands will not work until it is added. Do not create projects, add packages, or choose a target framework unless the task explicitly asks for it.

## Commands

Once the solution exists, run these from the repository root:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet format --verify-no-changes
```

A change is not finished until all four succeed.

## Architecture

The sample follows Clean Architecture. Dependencies point inward only.

| Project | Purpose | May reference |
|---|---|---|
| `Domain` | Entities, value objects, domain rules | Nothing |
| `Application` | Use cases, interfaces for external concerns | `Domain` |
| `Infrastructure` | EF Core, external services, implementations | `Application`, `Domain` |
| `Api` | HTTP endpoints, composition root | `Application`, and `Infrastructure` for dependency registration only |

Rules:

- `Domain` has no dependency on EF Core, ASP.NET Core, or any infrastructure package.
- `Application` defines interfaces. `Infrastructure` implements them.
- Endpoints stay thin: validate input, call the application layer, map the result. No business logic and no direct `DbContext` use in `Api`.
- If a change seems to need a dependency that points outward, stop and say so instead of adding the reference.

Layer-specific rules will live in an `AGENTS.md` inside each project folder. When one exists, it applies in addition to this file.

## Working rules

- Keep changes small and focused on the task. Do not refactor unrelated code.
- Read the surrounding code first and match its conventions.
- Add or update tests for every change in behavior.
- Never make a check pass by weakening it. Do not disable analyzers, add warning suppressions, lower severities, or skip, delete, or loosen tests to get a green build. If a rule seems wrong, say so and leave it in place.
- Do not add a NuGet package without stating why it is needed and what was considered instead.
- Never commit secrets, connection strings with credentials, or personal data. Use placeholders and document where real values come from.
- Do not invent APIs, package versions, or behavior. If you are not sure something exists in the current .NET version, check the official documentation or say that it is unverified.
- If instructions are ambiguous or conflict with these rules, ask before proceeding.

## Git workflow

Trunk-based development:

- `main` is protected and always in a working state. Never push to it directly.
- Make each change on a short-lived branch named by type, for example `feat/...`, `fix/...`, `docs/...`, `chore/...`, or `ci/...`.
- Open a pull request into `main`. Keep it small enough to review in one sitting.
- Write commit subjects in the imperative mood, such as "Add architecture tests".
- A human merges. Agents do not merge their own pull requests.

## Definition of done

- The build, tests, and format check pass.
- New behavior is covered by tests.
- No architecture rule above is broken.
- Documentation that the change affects is updated, including the README roadmap.
- The pull request description says what changed, why, and what was not verified.

## Documentation style

- Clear, plain English. Useful information over hype.
- Explain why a rule exists, not only what it is. Label opinions as opinions and link to sources.
- Do not use em-dashes. Use commas, periods, or parentheses.
- Do not claim the repository contains something it does not yet contain.

## Review guidelines

When reviewing a pull request in this repository:

- Report only problems you can point to in the diff, with the file and line.
- Order findings by severity: blocking, should fix, suggestion.
- If there is nothing worth raising, say so briefly. Do not invent findings.

Treat these as blocking:

- A dependency that breaks the architecture rules above.
- A weakened guardrail: a disabled analyzer, a new suppression, a skipped or deleted test, or a relaxed CI step, unless the pull request explains and justifies it.
- Secrets, credentials, or personal data in the diff.
- Anything listed as a security concern in `SECURITY.md`.
- Documentation or comments that describe something the code does not do.

Also check for:

- Bugs and unhandled edge cases.
- Changed behavior without tests.
- EF Core problems: N+1 queries, missing `AsNoTracking` on read-only queries, loading whole tables into memory, synchronous database calls in async code, and edits to migrations that have already been applied.
- Async problems: `.Result` or `.Wait()`, `async void`, and missing `CancellationToken` on I/O paths.
- New packages that are not justified in the pull request.
- Changes that are larger than the task required.
