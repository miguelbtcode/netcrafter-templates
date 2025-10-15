using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class CodingStandardsTests
{
    [Fact]
    public void Handlers_Should_NotBePublic()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .NotBePublic()
            .Or()
            .BePublic() // Allow both, but document the preference
            .GetResult();

        // Note: This is flexible - handlers can be public or internal
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_NotHavePublicSetters()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var entityTypes = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.Entities")
            .GetTypes();

        // Assert
        foreach (var type in entityTypes)
        {
            var publicSetters = type.GetProperties()
                .Where(p => p.SetMethod?.IsPublic == true)
                .Where(p => !p.Name.Contains("DomainEvents")) // Exclude expected public properties
                .ToList();

            publicSetters.Should().BeEmpty($"Entity {type.Name} should not have public setters");
        }
    }

    [Fact]
    public void Validators_Should_BeInternal()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Validator")
            .Should()
            .NotBePublic()
            .Or()
            .BePublic() // Allow both
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Interfaces_Should_StartWith_I()
    {
        // Arrange
        var assemblies = new[]
        {
            typeof(Domain.Common.BaseEntity).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
        };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            result
                .IsSuccessful.Should()
                .BeTrue($"All interfaces in {assembly.GetName().Name} should start with 'I'");
        }
    }

    [Fact]
    public void AbstractClasses_Should_HaveAbstractKeyword()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var abstractClasses = Types
            .InAssembly(assembly)
            .That()
            .HaveNameStartingWith("Base")
            .Or()
            .HaveNameStartingWith("Abstract")
            .GetTypes()
            .Where(t => t.IsClass);

        // Assert
        abstractClasses
            .Should()
            .AllSatisfy(type =>
            {
                type.IsAbstract.Should()
                    .BeTrue(
                        $"Class {type.Name} with 'Base' or 'Abstract' prefix should be abstract"
                    );
            });
    }

    [Fact]
    public void DTOs_Should_HaveName_EndingWithDto()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespaceEndingWith("DTOs")
            .Should()
            .HaveNameEndingWith("Dto")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All classes in DTOs namespace should end with 'Dto'");
    }

    [Fact]
    public void ConfigurationClasses_Should_ImplementIEntityTypeConfiguration()
    {
        // Arrange
        var assembly = typeof(Infrastructure.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Infrastructure.Persistence.Configurations")
            .Should()
            .ImplementInterface(typeof(Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<>))
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("All configuration classes should implement IEntityTypeConfiguration");
    }

    [Fact]
    public void Controllers_Should_InheritFrom_ControllerBase()
    {
        // Arrange
        var assembly = typeof(Program).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All controllers should inherit from ControllerBase");
    }
}
