using System.Reflection;
using Playbook.Domain.Todos;

namespace Playbook.ArchitectureTests;

/// <summary>
/// Enforces design rules from src/Domain/AGENTS.md that the compiler cannot check by itself.
/// </summary>
public sealed class DomainDesignTests
{
    private const string InitOnlyMarker = "System.Runtime.CompilerServices.IsExternalInit";

    private static readonly Assembly DomainAssembly = typeof(TodoItem).Assembly;

    [Fact]
    public void DomainTypesHaveNoPublicSetters()
    {
        // A public setter lets any caller change state without going through the rules of the entity.
        // Init-only properties are allowed, because they cannot change after the object is created.
        string[] violations = DomainAssembly.GetExportedTypes()
            .SelectMany(type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(property => property.SetMethod is { IsPublic: true } setter && !IsInitOnly(setter))
            .Select(property => $"{property.DeclaringType?.Name}.{property.Name}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Domain properties must not have a public setter: {string.Join(", ", violations)}");
    }

    private static bool IsInitOnly(MethodInfo setter)
    {
        return setter.ReturnParameter
            .GetRequiredCustomModifiers()
            .Any(modifier => modifier.FullName == InitOnlyMarker);
    }
}
