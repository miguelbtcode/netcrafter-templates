using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class NamingConventionTests
{
    [Fact]
    public void Controllers_Should_HaveName_EndingWithController()
    {
        // Arrange
        var assembly = typeof(Program).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("WebApi.Controllers")
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("All classes in Controllers namespace should end with 'Controller'");
    }

    [Fact]
    public void Handlers_Should_HaveName_EndingWithHandler()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespaceEndingWith("Commands")
            .Or()
            .ResideInNamespaceEndingWith("Queries")
            .And()
            .AreClasses()
            .And()
            .DoNotHaveNameEndingWith("Command")
            .And()
            .DoNotHaveNameEndingWith("Query")
            .And()
            .DoNotHaveNameEndingWith("Validator")
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All handler classes should end with 'Handler'");
    }

    [Fact]
    public void Commands_Should_HaveName_EndingWithCommand()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespaceEndingWith("Commands")
            .And()
            .AreNotNested()
            .And()
            .DoNotHaveNameEndingWith("Handler")
            .And()
            .DoNotHaveNameEndingWith("Validator")
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All command classes should end with 'Command'");
    }

    [Fact]
    public void Queries_Should_HaveName_EndingWithQuery()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespaceEndingWith("Queries")
            .And()
            .AreNotNested()
            .And()
            .DoNotHaveNameEndingWith("Handler")
            .And()
            .DoNotHaveNameEndingWith("Validator")
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All query classes should end with 'Query'");
    }

    [Fact]
    public void Validators_Should_HaveName_EndingWithValidator()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .Inherit(typeof(FluentValidation.AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All validator classes should end with 'Validator'");
    }

    [Fact]
    public void Entities_Should_NotHaveName_EndingWithEntity()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.Entities")
            .ShouldNot()
            .HaveNameEndingWith("Entity")
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue(
                "Entity classes should not have 'Entity' suffix (e.g., 'Product' not 'ProductEntity')"
            );
    }

    [Fact]
    public void Exceptions_Should_HaveName_EndingWithException()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace("Domain.Exceptions")
            .Should()
            .HaveNameEndingWith("Exception")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All exception classes should end with 'Exception'");
    }
}
