# Security

## Reporting a vulnerability

Please do not open a public issue for a security problem.

Email **info@dotnetdevs.io** with a description of the problem, the steps to reproduce it, and the affected files. You will get a reply as soon as possible. This project is maintained in spare time, so please allow a few days.

## Scope

This repository is a template. The code is a small sample meant to be copied and adapted, not a deployed service. The most useful reports are:

- A rule, guardrail, or sample that teaches or permits an insecure pattern.
- A workflow in `.github/workflows/` that could leak secrets or be abused from a fork.
- A dependency with a known vulnerability.

Only the latest state of the `main` branch is supported.

## Security review guidelines

Reviewers, human or AI, should treat the following as blocking in any pull request.

### Secrets and data

- API keys, tokens, passwords, or connection strings with credentials anywhere in the diff, including tests, samples, and documentation.
- Real personal data in seed data, fixtures, or logs.
- Logging of secrets, tokens, full request bodies, or personal data.

### Data access

- SQL built by concatenating or interpolating user input, for example `FromSqlRaw` or `ExecuteSqlRaw` with an interpolated string. Use parameters or the interpolated-safe APIs.
- Endpoints that return entities directly and expose fields the caller should not see.
- Queries that trust a client-supplied identifier without checking that the caller may access that record.

### API

- Endpoints added without a deliberate decision on authentication and authorization.
- Input bound from requests without validation.
- Error responses that expose stack traces or internal details.
- CORS or HTTPS settings loosened without explanation.

### Dependencies and build

- New packages from unknown publishers, or packages added without a stated reason.
- Package versions with known vulnerabilities.
- Disabled security analyzers or suppressed security warnings.

### GitHub workflows

- Use of `pull_request_target`, or any workflow that runs untrusted pull request code with access to secrets.
- Workflow permissions broader than the job needs.
- Third-party actions that are not pinned to a version.
- Untrusted input, such as a pull request title or branch name, interpolated directly into a shell command.
