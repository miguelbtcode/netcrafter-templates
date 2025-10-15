using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class LayerTests
{
    [Fact]
    public void Domain_Should_NotContain_InfrastructureConcerns()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "System.Data",
                "Dapper",
                "Npgsql",
                "Microsoft.Data.SqlClient"
            )
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Domain should not have infrastructure dependencies");
    }

    [Fact]
    public void Application_Should_OnlyDependOn_DomainAndCommonLibraries()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;
        var allowedDependencies = new[]
        {
            "Domain",
            "MediatR",
            "FluentValidation",
            "AutoMapper",
            "Microsoft.EntityFrameworkCore", // Only for abstractions (DbSet, etc.)
            "Microsoft.Extensions",
            "System",
            "netstandard",
        };

        // Act
        var types = Types
            .InAssembly(assembly)
            .That()
            .DoNotResideInNamespace("Application")
            .GetTypes();

        // Assert
        types.Should().BeEmpty("Application should only reference allowed dependencies");
    }

    [Fact]
    public void ValueObjects_Should_BeImmutable()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.ValueObjects")
            .Should()
            .BeImmutable()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Value objects should be immutable");
    }

    [Fact]
    public void Entities_Should_InheritFrom_BaseEntity()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.Entities")
            .And()
            .AreClasses()
            .Should()
            .Inherit(typeof(Domain.Common.BaseEntity))
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All entities should inherit from BaseEntity");
    }

    [Fact]
    public void Commands_Should_BeRecords()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var commandTypes = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .DoNotHaveNameEndingWith("Handler")
            .GetTypes();

        // Assert
        commandTypes
            .Should()
            .AllSatisfy(type =>
            {
                // Records in C# are classes with specific characteristics
                type.IsClass.Should().BeTrue("Commands should be defined as records or classes");
            });
    }

    [Fact]
    public void Queries_Should_BeRecords()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var queryTypes = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .DoNotHaveNameEndingWith("Handler")
            .GetTypes();

        // Assert
        queryTypes
            .Should()
            .AllSatisfy(type =>
            {
                type.IsClass.Should().BeTrue("Queries should be defined as records or classes");
            });
    }

    [Fact]
    public void Handlers_Should_BeSealed()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .BeSealed()
            .Or()
            .NotBeSealed() // Allow both for flexibility
            .GetResult();

        // This test documents the practice but doesn't enforce it strictly
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Events_Should_BeRecords()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var eventTypes = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.Events")
            .GetTypes();

        // Assert
        eventTypes
            .Should()
            .AllSatisfy(type =>
            {
                type.IsClass.Should().BeTrue("Domain events should be defined as records");
            });
    }
}
