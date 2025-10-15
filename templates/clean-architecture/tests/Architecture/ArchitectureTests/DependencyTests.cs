using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class DependencyTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string WebApiNamespace = "WebApi";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Domain.Common.BaseEntity).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("Domain layer should not have dependencies on other layers");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_InfrastructureOrWebApi()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("Application layer should not have dependencies on Infrastructure or WebApi");
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOn_WebApi()
    {
        // Arrange
        var assembly = typeof(Infrastructure.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(WebApiNamespace)
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("Infrastructure layer should not have dependencies on WebApi");
    }

    [Fact]
    public void Handlers_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("All handlers should depend on MediatR");
    }

    [Fact]
    public void Controllers_Should_NotHaveDependencyOn_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Program).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result
            .IsSuccessful.Should()
            .BeTrue("Controllers should not have direct dependencies on Infrastructure");
    }
}
