# Integration Tests with Testcontainers

This project contains integration tests for the WebApi using **Testcontainers** for database isolation.

## Overview

Integration tests verify that different components of the application work together correctly. This includes:
- HTTP endpoints
- Database operations
- MediatR pipeline (handlers, validators, behaviors)
- Entity Framework Core configurations

## Key Features

### 1. **Testcontainers**
- Automatically spins up a real SQL Server container for each test run
- Ensures tests run against a real database (not in-memory)
- Provides complete isolation between test runs
- Automatically cleans up containers after tests

### 2. **Respawn**
- Resets database state between tests
- Faster than recreating the database
- Maintains referential integrity

### 3. **Base Test Infrastructure**

#### IntegrationTestWebAppFactory
- Configures the test server
- Replaces production database with Testcontainers database
- Ensures database is created before tests run

#### BaseIntegrationTest
- Provides common setup for all integration tests
- Exposes `HttpClient` for making HTTP requests
- Exposes `DbContext` for database assertions
- Handles database cleanup between tests

## Prerequisites

- Docker must be running on your machine
- .NET 9.0 SDK

## Running Tests

```bash
# Ensure Docker is running
docker ps

# Run all integration tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~CreateCategoryTests"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Writing New Tests

### Example Test

```csharp
public class MyFeatureTests : BaseIntegrationTest
{
    public MyFeatureTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task MyTest_ShouldWork()
    {
        // Arrange
        var request = new { /* data */ };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/endpoint", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database
        var entity = await DbContext.MyEntities.FirstOrDefaultAsync();
        entity.Should().NotBeNull();
    }
}
```

## Best Practices

1. **Inherit from BaseIntegrationTest**: Always inherit from `BaseIntegrationTest` to get database cleanup
2. **Use FluentAssertions**: For readable and expressive assertions
3. **Test HTTP endpoints**: Test through the API layer, not directly against handlers
4. **Verify in database**: Always verify state changes in the database
5. **Keep tests isolated**: Each test should be independent and not rely on other tests

## Troubleshooting

### Docker not running
```
Error: Cannot connect to Docker daemon
```
**Solution**: Start Docker Desktop

### Port conflicts
```
Error: Port already in use
```
**Solution**: Testcontainers automatically assigns random ports. This shouldn't happen, but if it does, restart Docker.

### Slow tests
- First run is slow because it downloads the SQL Server image
- Subsequent runs are faster as the image is cached
- Each test run takes ~10-15 seconds to spin up the container

## Package References

- **Testcontainers** (3.10.0): Container orchestration
- **Testcontainers.MsSql** (3.10.0): SQL Server specific support
- **Respawn** (6.2.1): Database cleanup
- **FluentAssertions** (6.12.0): Assertion library
- **Microsoft.AspNetCore.Mvc.Testing** (9.0.0): WebApplicationFactory
