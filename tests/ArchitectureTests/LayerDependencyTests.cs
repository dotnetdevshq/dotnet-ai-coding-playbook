using System.Reflection;
using Playbook.Application.Todos;
using Playbook.Domain.Todos;
using Playbook.Infrastructure;

namespace Playbook.ArchitectureTests;

/// <summary>
/// Enforces the dependency rule from AGENTS.md: dependencies point inward only.
/// These tests fail the build when a layer starts using something it must not use,
/// whether or not the author read the rules.
/// </summary>
public sealed class LayerDependencyTests
{
    private const string EntityFramework = "Microsoft.EntityFrameworkCore";
    private const string AspNetCore = "Microsoft.AspNetCore";
    private const string MicrosoftExtensions = "Microsoft.Extensions";

    private static readonly Assembly DomainAssembly = typeof(TodoItem).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(TodoService).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(InfrastructureServiceRegistration).Assembly;

    [Fact]
    public void DomainUsesNoOtherLayerAndNoFramework()
    {
        AssertUsesNone(DomainAssembly, "Playbook.", EntityFramework, AspNetCore, MicrosoftExtensions);
    }

    [Fact]
    public void ApplicationDoesNotUseInfrastructureApiOrFrameworks()
    {
        AssertUsesNone(ApplicationAssembly, "Playbook.Infrastructure", "Playbook.Api", EntityFramework, AspNetCore);
    }

    [Fact]
    public void InfrastructureDoesNotUseApiOrAspNetCore()
    {
        AssertUsesNone(InfrastructureAssembly, "Playbook.Api", AspNetCore);
    }

    private static void AssertUsesNone(Assembly assembly, params string[] forbiddenPrefixes)
    {
        // The compiler only records references that the code really uses,
        // so this list shows what the layer depends on in practice.
        string[] violations = assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .Where(name => forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal)))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"{assembly.GetName().Name} must not depend on: {string.Join(", ", violations)}");
    }
}
