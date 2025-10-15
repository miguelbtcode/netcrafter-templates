# Clean Architecture Template

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Enterprise-ready .NET solution template implementing **Clean Architecture** with **Domain-Driven Design (DDD)**, **CQRS**, and comprehensive testing infrastructure.

## Features

- ✅ **Clean Architecture** - Dependency inversion with clear separation of concerns
- ✅ **CQRS + MediatR** - Command Query Responsibility Segregation pattern
- ✅ **Domain-Driven Design** - Rich domain models with entities, value objects, and domain events
- ✅ **FluentValidation** - Declarative validation with pipeline behaviors
- ✅ **Entity Framework Core** - Modern ORM with SQLite (development-ready)
- ✅ **Structured Logging** - Serilog with console and file outputs
- ✅ **Docker Support** - Multi-stage Dockerfile and docker-compose
- ✅ **API Documentation** - Swagger/OpenAPI integration
- ✅ **Health Checks** - Built-in health monitoring endpoint
- ✅ **Complete Test Suite** - Unit, Integration, Architecture, and E2E tests
- ✅ **Code Quality** - SonarAnalyzer and EditorConfig standards

## Project Structure

```
├── src/
│   ├── Domain/                    # Enterprise business rules (zero dependencies)
│   │   ├── Entities/              # Domain entities with business logic
│   │   ├── Events/                # Domain events for cross-aggregate communication
│   │   └── Common/                # Shared domain primitives
│   │
│   ├── Application/               # Application business rules
│   │   ├── Common/                # Interfaces, behaviors, models
│   │   ├── Categories/            # Feature: Category management (CQRS)
│   │   └── Products/              # Feature: Product management (CQRS)
│   │
│   ├── Infrastructure/            # External concerns
│   │   ├── Persistence/           # EF Core, DbContext, configurations
│   │   └── DependencyInjection.cs # Service registration
│   │
│   └── Presentation/              # API layer
│       ├── Controllers/           # REST API endpoints
│       ├── Middleware/            # Global exception handling
│       └── Program.cs             # Application entry point
│
├── tests/
│   ├── Unit/
│   │   ├── Domain.UnitTests/      # Domain logic tests
│   │   └── Application.UnitTests/ # Application logic tests
│   ├── Integration/
│   │   └── Presentation.IntegrationTests/  # API integration tests
│   ├── E2E/
│   │   └── Presentation.E2ETests/ # End-to-end workflow tests
│   └── Architecture/
│       └── ArchitectureTests/     # Architecture rules enforcement
│
├── docker-compose.yml             # Container orchestration
├── Dockerfile                     # Multi-stage optimized build
└── .editorconfig                  # Code style enforcement
```

## Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional)

### Using Docker Compose (Recommended)

```bash
# Start all services
docker-compose up -d

# Access the application
# API:     http://localhost:5000
# Swagger: http://localhost:5000/swagger
# Health:  http://localhost:5000/health
```

### Running Locally

```bash
# Restore dependencies
dotnet restore

# Run the API
cd src/Presentation
dotnet run

# Run all tests
dotnet test
```

### Database Setup

The template uses **SQLite** by default for easy development setup. The database file is created automatically on first run.

```bash
# Apply migrations (if needed)
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation

# Create a new migration
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Presentation
```

## Architecture Principles

### Clean Architecture Layers

1. **Domain** - Core business logic, entities, and domain events (no dependencies)
2. **Application** - Use cases, CQRS handlers, DTOs, and interfaces
3. **Infrastructure** - Database, file system, external APIs (implements Application interfaces)
4. **Presentation** - Controllers, middleware, API contracts

### CQRS Pattern

Commands and Queries are separated for better scalability and maintainability:

```csharp
// Command (writes data)
public record CreateCategoryCommand(string Name, string Description)
    : IRequest<Result<Guid>>;

// Query (reads data)
public record GetCategoryByIdQuery(Guid Id)
    : IRequest<Result<CategoryDto>>;
```

### Result Pattern

Explicit error handling without exceptions:

```csharp
public async Task<IActionResult> Create(CreateCategoryCommand command)
{
    var result = await _mediator.Send(command);

    return result.IsSuccess
        ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
        : BadRequest(result.Error);
}
```

## Testing

The template includes a comprehensive test suite:

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Unit/Domain.UnitTests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Test Types

- **Unit Tests** - Fast, isolated tests for domain and application logic
- **Integration Tests** - API tests with in-memory SQLite database
- **E2E Tests** - Complete workflow tests simulating real user scenarios
- **Architecture Tests** - Enforce architectural rules and dependencies

## API Documentation

Swagger UI is available in development mode:

```
http://localhost:5000/swagger
```

### Example Endpoints

```http
POST   /api/categories          # Create a category
GET    /api/categories/{id}     # Get category by ID
POST   /api/products            # Create a product
GET    /api/products/{id}       # Get product by ID
GET    /health                  # Health check
```

## Docker

### Build and Run

```bash
# Build the image
docker build -t cleanarch-api .

# Run the container
docker run -p 5000:8080 cleanarch-api
```

### Docker Compose Services

- **presentation** - The .NET API application
- **sqlite-data** - Persistent volume for SQLite database
- **app-logs** - Persistent volume for application logs

## Code Quality

### Static Analysis

All projects include **SonarAnalyzer.CSharp** for continuous code quality monitoring:

- Code smells detection
- Bug prevention
- Security vulnerability scanning
- Code complexity analysis

### Coding Standards

**EditorConfig** enforces consistent coding standards:

- Naming conventions (PascalCase for public, camelCase for parameters, \_camelCase for private fields)
- Code style (braces, spacing, indentation)
- Modern C# features (var, expression bodies, pattern matching)

## Project Configuration

### Key Files

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `.editorconfig` - Code style rules
- `docker-compose.yml` - Container orchestration
- `Dockerfile` - Multi-stage optimized build

### Environment Variables

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Data Source=CleanArchitecture.db
```

## Contributing

This template is designed to be a solid foundation for enterprise applications. Feel free to:

- Add new features following the CQRS pattern
- Extend domain entities with rich business logic
- Implement additional infrastructure concerns
- Add more comprehensive tests

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Library](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

**Built with ❤️ using .NET 9**
