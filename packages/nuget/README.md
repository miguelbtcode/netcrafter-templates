# NetCrafter Templates

Professional .NET architecture templates for enterprise applications using Clean Architecture, DDD, and CQRS patterns.

## � Installation

```bash
dotnet new install NetCrafter.Templates
```

## � Quick Start

```bash
# Create a new Clean Architecture project
dotnet new clean-arch -n MyProject

# Run the application
cd MyProject
dotnet run --project src/Presentation
```

Visit http://localhost:5000/swagger for API documentation.

## 📋 Available Templates

### Clean Architecture (`clean-arch`)

Enterprise-ready template with Domain-Driven Design and CQRS.

**Parameters:**

- `--framework` - Target framework (net8.0, net9.0) [default: net9.0]
- `--database` - Database provider (SqlServer, PostgreSQL, SQLite) [default: SqlServer]
- `--docker` - Include Docker support [default: true]
- `--tests` - Include test projects [default: true]

## 🏗️ Project Structure

```
MyProject/
├── src/
│   ├── Domain/          # Business entities & rules
│   ├── Application/     # CQRS commands & queries
│   ├── Infrastructure/  # Data access & external services
│   └── Presentation/    # API controllers & endpoints
└── tests/
    ├── Unit/            # Unit tests
    ├── Integration/     # Integration tests
    └── Architecture/    # Architecture validation
```

## ✨ Features

- ✅ Clean Architecture with DDD
- ✅ CQRS with MediatR
- ✅ Entity Framework Core
- ✅ Multiple database support
- ✅ Comprehensive testing
- ✅ Docker & docker-compose
- ✅ Swagger/OpenAPI
- ✅ Health checks
- ✅ Structured logging

## � Documentation

Full documentation: https://github.com/miguelbtcode/netcrafter-templates

## 🐛 Issues & Support

Report issues: https://github.com/miguelbtcode/netcrafter-templates/issues

## 📄 License

MIT License
