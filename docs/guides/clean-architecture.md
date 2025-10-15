# Clean Architecture Guide

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture Principles](#architecture-principles)
- [Project Structure](#project-structure)
- [Layer Responsibilities](#layer-responsibilities)
- [Key Patterns & Practices](#key-patterns--practices)
- [Getting Started](#getting-started)
- [Common Scenarios](#common-scenarios)
- [Best Practices](#best-practices)
- [Testing Strategy](#testing-strategy)

---

## Overview

Clean Architecture is an enterprise-grade architectural pattern that emphasizes separation of concerns, maintainability, and testability. It organizes code into concentric layers where dependencies flow inward toward the core domain logic.

### Core Principles

1. **Independence of Frameworks** - Business logic doesn't depend on external libraries
2. **Testability** - Business rules can be tested without UI, database, or external services
3. **Independence of UI** - UI can change without affecting business rules
4. **Independence of Database** - Business rules aren't bound to a specific database
5. **Independence of External Services** - Business rules don't know about external dependencies

### Benefits

- ✅ **Maintainability** - Clear separation makes code easier to maintain
- ✅ **Testability** - Business logic is isolated and easily testable
- ✅ **Flexibility** - Easy to swap implementations (database, UI, etc.)
- ✅ **Scalability** - Well-organized structure supports team growth
- ✅ **Domain Focus** - Business logic is centralized and protected

---

## Architecture Principles

### The Dependency Rule

> **Dependencies must point inward**. Nothing in an inner circle can know anything about an outer circle.

```
┌─────────────────────────────────────┐
│     Presentation (Web API)          │  ← Framework & Drivers
├─────────────────────────────────────┤
│     Infrastructure                  │  ← Interface Adapters
├─────────────────────────────────────┤
│     Application (Use Cases)         │  ← Application Business Rules
├─────────────────────────────────────┤
│     Domain (Entities & Rules)       │  ← Enterprise Business Rules
└─────────────────────────────────────┘
         ↓ Dependencies flow inward
```

---

## Project Structure

```
CleanArchitecture/
├── src/
│   ├── Domain/                      # Enterprise Business Rules
│   │   ├── Entities/                # Domain entities
│   │   ├── ValueObjects/            # Value objects
│   │   ├── Events/                  # Domain events
│   │   ├── Exceptions/              # Domain exceptions
│   │   ├── Enums/                   # Domain enums
│   │   └── Common/                  # Shared domain abstractions
│   │
│   ├── Application/                 # Application Business Rules
│   │   ├── Common/
│   │   │   ├── Interfaces/          # Application interfaces
│   │   │   ├── Behaviours/          # Pipeline behaviors
│   │   │   ├── Mappings/            # Object mappings
│   │   │   └── Models/              # DTOs, Result types
│   │   └── [Features]/              # Feature folders (CQRS)
│   │       ├── Commands/            # Command handlers
│   │       ├── Queries/             # Query handlers
│   │       └── DTOs/                # Feature-specific DTOs
│   │
│   ├── Infrastructure/              # Interface Adapters
│   │   ├── Persistence/             # Database implementation
│   │   │   ├── Configurations/      # EF Core configurations
│   │   │   ├── Repositories/        # Repository implementations
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Identity/                # Authentication/Authorization
│   │   ├── Services/                # External service implementations
│   │   └── DependencyInjection.cs   # Infrastructure DI setup
│   │
│   └── Presentation/                # Frameworks & Drivers
│       ├── Controllers/             # API Controllers
│       ├── Middleware/              # HTTP middleware
│       ├── Program.cs               # Application entry point
│       └── appsettings.json         # Configuration
│
└── tests/
    ├── Unit/
    │   ├── Domain.UnitTests/        # Domain logic tests
    │   └── Application.UnitTests/   # Application logic tests
    ├── Integration/
    │   └── Presentation.IntegrationTests/  # Integration tests
    ├── E2E/
    │   └── Presentation.E2ETests/   # End-to-end tests
    └── Architecture/
        └── ArchitectureTests/       # Architecture rule tests
```

---

## Layer Responsibilities

### 1. Domain Layer (Core)

**Purpose:** Contains enterprise business logic and rules that are independent of any application use case.

**Responsibilities:**

- Define business entities
- Implement domain logic and invariants
- Define value objects
- Raise domain events
- Define domain exceptions

**Key Components:**

```csharp
// Entities - Business objects with identity
public class Product : BaseEntity
{
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public ProductStatus Status { get; private set; }

    private Product() { } // EF Core

    public static Product Create(string name, Money price)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Status = ProductStatus.Draft
        };

        product.AddDomainEvent(new ProductCreatedEvent(product.Id));
        return product;
    }

    public void Publish()
    {
        if (Status != ProductStatus.Draft)
            throw new DomainException("Only draft products can be published");

        Status = ProductStatus.Published;
    }
}

// Value Objects - Objects without identity
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        return new Money { Amount = amount, Currency = currency };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

**Rules:**

- ❌ No dependencies on outer layers
- ❌ No framework dependencies
- ✅ Pure C# classes
- ✅ Business rules and validation

---

### 2. Application Layer

**Purpose:** Contains application-specific business rules and orchestrates the flow of data between the domain and infrastructure layers.

**Responsibilities:**

- Define application use cases
- Implement CQRS commands and queries
- Define application interfaces (repositories, services)
- Coordinate domain objects to perform tasks
- Handle cross-cutting concerns (validation, logging, transactions)

**Key Components:**

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price, string Currency)
    : IRequest<Result<Guid>>;

// Command Handler
public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var money = Money.Create(request.Price, request.Currency);
        var product = Product.Create(request.Name, money);

        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}

// Pipeline Behavior - Validation
public class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

**Rules:**

- ✅ Depends on Domain layer
- ✅ Defines interfaces implemented by Infrastructure
- ❌ No dependencies on Infrastructure or Presentation
- ✅ Uses MediatR for CQRS pattern

---

### 3. Infrastructure Layer

**Purpose:** Implements interfaces defined in the Application layer and provides technical capabilities.

**Responsibilities:**

- Implement data access (repositories)
- Implement external service integrations
- Configure database context and mappings
- Implement authentication/authorization
- Provide caching, logging, etc.

**Key Components:**

```csharp
// Repository Implementation
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }
}

// EF Core Configuration
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(p => p.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.Property(p => p.Status)
            .HasConversion<string>();
    }
}
```

**Rules:**

- ✅ Implements interfaces from Application layer
- ✅ Depends on Domain and Application layers
- ❌ No business logic
- ✅ Framework-specific implementations

---

### 4. Presentation Layer

**Purpose:** Provides the user interface and handles HTTP requests/responses.

**Responsibilities:**

- Define API endpoints
- Handle HTTP requests and responses
- Validate input
- Handle exceptions
- Provide API documentation

**Key Components:**

```csharp
// API Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Price,
            request.Currency);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound();
    }
}

// Global Exception Handler
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            DomainException => (StatusCodes.Status400BadRequest, "Domain Error"),
            _ => (StatusCodes.Status500InternalServerError, "Server Error")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            title,
            status = statusCode,
            detail = exception.Message
        }, cancellationToken);

        return true;
    }
}
```

**Rules:**

- ✅ Depends on Application layer only
- ❌ No direct dependencies on Infrastructure or Domain
- ✅ Uses dependency injection
- ✅ Thin controllers (delegate to Application layer)

---

## Key Patterns & Practices

### 1. CQRS (Command Query Responsibility Segregation)

Separates read and write operations for better scalability and maintainability.

```csharp
// Command - Modifies state
public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;

// Query - Reads state
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
```

### 2. Repository Pattern

Abstracts data access logic and provides a collection-like interface.

```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Update(Product product);
    void Delete(Product product);
}
```

### 3. Unit of Work

Maintains a list of objects affected by a business transaction and coordinates the writing out of changes.

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4. Domain Events

Decouple components by using events to communicate between aggregates.

```csharp
public class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; }

    public ProductCreatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}
```

### 5. Result Pattern

Handle errors without exceptions for flow control.

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }

    public static Result<T> Success(T value) => new(true, value, Error.None);
    public static Result<T> Failure(Error error) => new(false, default, error);
}
```

---

## Getting Started

### 1. Create a New Project

```bash
dotnet new clean-arch -n MyProject
cd MyProject
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Update Database Connection

Edit `appsettings.json` in the Presentation project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyProjectDb;Trusted_Connection=true;"
  }
}
```

### 4. Apply Migrations

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation
```

### 5. Run the Application

```bash
dotnet run --project src/Presentation
```

### 6. Test the API

Navigate to `https://localhost:5001/swagger` to explore the API.

---

## Common Scenarios

### Adding a New Entity

1. **Create Entity in Domain Layer**

```csharp
// Domain/Entities/Category.cs
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public static Category Create(string name, string description)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }
}
```

2. **Create Repository Interface in Application**

```csharp
// Application/Common/Interfaces/ICategoryRepository.cs
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
}
```

3. **Implement Repository in Infrastructure**

```csharp
// Infrastructure/Persistence/Repositories/CategoryRepository.cs
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Categories.FindAsync(new object[] { id }, ct);

    public async Task AddAsync(Category category, CancellationToken ct)
        => await _context.Categories.AddAsync(category, ct);
}
```

4. **Configure EF Core Mapping**

```csharp
// Infrastructure/Persistence/Configurations/CategoryConfiguration.cs
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
    }
}
```

5. **Create Command/Query**

```csharp
// Application/Categories/Commands/CreateCategoryCommand.cs
public record CreateCategoryCommand(string Name, string Description)
    : IRequest<Result<Guid>>;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.Description);
        await _repository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(category.Id);
    }
}
```

6. **Add API Endpoint**

```csharp
// Presentation/Controllers/CategoriesController.cs
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryRequest request,
        CancellationToken ct)
    {
        var command = new CreateCategoryCommand(request.Name, request.Description);
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Created() : BadRequest(result.Error);
    }
}
```

---

## Best Practices

### Domain Layer

- ✅ Keep entities focused on business logic
- ✅ Use value objects for concepts without identity
- ✅ Make constructors private, use factory methods
- ✅ Protect invariants in entity methods
- ✅ Raise domain events for important business changes
- ❌ Don't use primitive obsession
- ❌ Don't expose setters publicly

### Application Layer

- ✅ One handler per use case
- ✅ Use DTOs for data transfer
- ✅ Implement cross-cutting concerns with behaviors
- ✅ Keep handlers focused and simple
- ✅ Use the Result pattern for operation outcomes
- ❌ Don't put business logic in handlers
- ❌ Don't depend on Infrastructure directly

### Infrastructure Layer

- ✅ Keep repository methods simple
- ✅ Use EF Core configurations for mappings
- ✅ Implement interfaces defined in Application
- ✅ Handle technical concerns (caching, logging)
- ❌ Don't put business logic in repositories
- ❌ Don't expose IQueryable to Application layer

### Presentation Layer

- ✅ Keep controllers thin
- ✅ Validate input at the boundary
- ✅ Use proper HTTP status codes
- ✅ Provide clear error messages
- ✅ Use API versioning
- ❌ Don't put business logic in controllers
- ❌ Don't access database directly

---

## Testing Strategy

### Unit Tests

Test business logic in isolation.

```csharp
[Fact]
public void Product_Create_ShouldRaiseDomainEvent()
{
    // Arrange
    var name = "Test Product";
    var price = Money.Create(100, "USD");

    // Act
    var product = Product.Create(name, price);

    // Assert
    product.DomainEvents.Should().ContainSingle(e => e is ProductCreatedEvent);
}
```

### Integration Tests

Test application use cases with real infrastructure.

```csharp
[Fact]
public async Task CreateProduct_ShouldPersistToDatabase()
{
    // Arrange
    var command = new CreateProductCommand("Test", 100, "USD");

    // Act
    var result = await _mediator.Send(command);

    // Assert
    var product = await _context.Products.FindAsync(result.Value);
    product.Should().NotBeNull();
    product.Name.Should().Be("Test");
}
```

### Architecture Tests

Enforce architectural rules.

```csharp
[Fact]
public void Domain_ShouldNotDependOnOtherLayers()
{
    var result = Types.InAssembly(DomainAssembly)
        .ShouldNot()
        .HaveDependencyOnAll(
            ApplicationAssembly.GetName().Name,
            InfrastructureAssembly.GetName().Name,
            PresentationAssembly.GetName().Name)
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}
```

---

## Summary

Clean Architecture provides a robust foundation for building maintainable, testable, and scalable applications. By following the dependency rule and organizing code into well-defined layers, you create systems that are:

- **Easy to understand** - Clear separation of concerns
- **Easy to test** - Business logic is isolated
- **Easy to change** - Loose coupling between layers
- **Easy to scale** - Well-organized for team growth

Start with the domain, build your use cases in the application layer, implement technical details in infrastructure, and expose functionality through the presentation layer.

---

**Happy Coding! 🚀**
