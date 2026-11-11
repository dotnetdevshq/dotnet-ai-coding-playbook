# .NET AI Coding Playbook

Rules, guardrails, and a template that keep AI-written .NET code up to your standards.

> **Status: early work in progress.** This repository was started in November 2026. The root rules, build guardrails, a solution skeleton, data access with EF Core, and the rules for the Infrastructure and API layers are in place. The rules for the other layers are still planned. See [Roadmap](#roadmap) for what exists today.

## Why this exists

AI coding agents write .NET code quickly. Without clear rules and automatic checks, they also break layer boundaries, write inefficient queries, skip tests, and drift away from the conventions of the codebase they are working in.

This playbook treats that as an engineering problem, not a prompting problem. It combines two things:

1. **Rules the agent reads**, so it knows how the codebase is meant to be built.
2. **Guardrails the build enforces**, so a violation fails whether or not the agent read the rules.

## What it will contain

- **Agent instructions in `AGENTS.md`.** One root file with the rules that apply everywhere, plus one file per layer with the rules for that layer only.
- **Guardrails that work without any AI tool.** `.editorconfig`, analyzers with warnings treated as errors, architecture tests, and a CI workflow.
- **A small Clean Architecture sample solution.** Domain, Application, Infrastructure (EF Core), and API projects with unit, integration, and architecture tests, so every rule has real code to apply to.
- **The reasoning behind each rule**, in `docs/`, so you can decide which rules fit your team and adapt the rest.

## Principles

- **Tool-agnostic.** The playbook uses [`AGENTS.md`](https://agents.md), an open format read by many AI coding tools. It does not favor or depend on any single tool.
- **Enforce, don't just instruct.** A rule that matters is backed by an analyzer, a test, or a build gate wherever that is possible.
- **Small and adaptable.** The sample is deliberately small. Copy what helps and change what does not fit.
- **Explained, not just listed.** Every rule comes with its reason. Opinions are labeled as opinions, and sources are linked.

## Roadmap

| Step | Status |
|---|---|
| Repository setup and README | Done |
| Root `AGENTS.md`, `SECURITY.md`, license, `.editorconfig` | Done |
| Build guardrails and CI workflow | Done |
| Clean Architecture sample solution (skeleton) | Done |
| .NET 10 and .NET 11 multi-targeting | Done |
| Data access with EF Core (SQLite) and its rules in `src/Infrastructure/AGENTS.md` | Done |
| API layer rules in `src/Api/AGENTS.md` | Done |
| Application and domain, and testing rules | Planned |
| UI layer (optional variant) | Later |

A companion guide is planned for [dotnetdevs.io](https://dotnetdevs.io).

## Contributing

The project is not ready for outside contributions yet. Once the first version is in place, issues and pull requests will be welcome.

Branching follows trunk-based development:

- `main` is the trunk and should always be in a working state.
- Every change is made on a short-lived branch and merged into `main` through a pull request.
- Branches are small and merged quickly. There are no long-lived development or release branches.

## License

Released under the [MIT License](LICENSE).

## About

Maintained by [dotnetdevs.io](https://dotnetdevs.io), home of the .NET Developers community and The .NET Insider newsletter.
