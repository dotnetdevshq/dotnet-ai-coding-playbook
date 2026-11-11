using System.Reflection;
using Xunit.Sdk;

namespace Playbook.TestConventions;

/// <summary>
/// Conventions that every test project must follow. This file is compiled into each test project
/// (see tests/Directory.Build.props), so each project checks its own tests.
/// </summary>
public sealed class TestConventionTests
{
    [Fact]
    public void NoTestIsSkipped()
    {
        // A skipped test looks like coverage but checks nothing. Fix the test or the code instead.
        string[] skipped = typeof(TestConventionTests).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(IsSkipped)
            .Select(method => $"{method.DeclaringType?.Name}.{method.Name}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            skipped.Length == 0,
            $"Tests must not be skipped: {string.Join(", ", skipped)}");
    }

    private static bool IsSkipped(MethodInfo method)
    {
        // Skip can be set on the test itself ([Fact] or [Theory]) or on one of its data rows ([InlineData]).
        return method.GetCustomAttributes<FactAttribute>().Any(fact => fact.Skip is not null)
            || method.GetCustomAttributes<DataAttribute>().Any(data => data.Skip is not null);
    }
}
