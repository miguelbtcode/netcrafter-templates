# NetCrafter Templates 🏗️

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Documentation](https://img.shields.io/badge/docs-available-blue.svg)](docs/guides/)

> **Professional, enterprise-ready .NET architecture templates for modern application development**

NetCrafter Templates provides a curated collection of battle-tested architectural patterns for .NET applications. Each template is designed with best practices, SOLID principles, and real-world scalability in mind.

---

## ✨ Features

- 🎯 **Production-Ready** - Battle-tested patterns used in enterprise applications
- 🏛️ **Multiple Architectures** - Choose the right pattern for your needs
- 📦 **Easy Installation** - Simple dotnet CLI integration
- 🧪 **Test Coverage** - Unit, Integration, and E2E tests included
- 📚 **Comprehensive Docs** - Detailed documentation for each template
- 🔄 **Active Maintenance** - Regular updates and improvements

---

## 🏗️ Available Templates

| Template               | Description                                    | Best For                                  |
| ---------------------- | ---------------------------------------------- | ----------------------------------------- |
| **Clean Architecture** | Enterprise-grade layered architecture with DDD | Large, complex domain-driven applications |
| **Vertical Slice**     | Feature-focused architecture with Carter       | CQRS-based microservices and APIs         |
| **Modular Monolith**   | Modular architecture with domain events        | Scalable monoliths with clear boundaries  |
| **Onion Architecture** | Dependency-inversion concentric layers         | Domain-centric applications               |
| **Serverless**         | Azure Functions-based architecture             | Event-driven, cloud-native solutions      |

---

## 🚀 Quick Start

### Installation

```bash
# Install from NuGet (coming soon)
dotnet new install NetCrafter.Templates

# Or install from source
git clone https://github.com/yourusername/netcrafter-templates.git
cd netcrafter-templates
```

### Usage

```bash
# Create a new project with Clean Architecture
dotnet new clean-arch -n MyAwesomeProject

# Create with Vertical Slice Architecture
dotnet new vertical-slice -n MyApiProject

# Create with Modular Monolith
dotnet new modular-monolith -n MyModularApp

# Create with Onion Architecture
dotnet new onion-arch -n MyDomainApp

# Create with Serverless Architecture
dotnet new serverless-arch -n MyFunctionsApp
```

---

## 📋 Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) or higher
- (Optional) [Docker](https://www.docker.com/) for containerization
- (Optional) IDE: Visual Studio 2022, VS Code, or JetBrains Rider

---

## 📚 Documentation

### Template-Specific Guides

- 📖 [Clean Architecture](./docs/guides/clean-architecture.md) - Layered DDD approach
- 📖 [Vertical Slice Architecture](./templates/vertical-slice/README.md) - Feature-based slices
- 📖 [Modular Monolith](./templates/modular-monolith/README.md) - Modular design
- 📖 [Onion Architecture](./templates/onion-architecture/README.md) - Concentric layers
- 📖 [Serverless Architecture](./templates/serverless/README.md) - Cloud-native functions

### Additional Resources

- 📘 [Architecture Diagrams](./docs/diagrams/)
- 📘 [Best Practices Guide](./docs/guides/)

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for a detailed history of changes.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Inspired by the .NET community's best practices
- Built with ❤️ for developers who care about architecture

---

## 📧 Support

- 🐛 [Report Issues](https://github.com/yourusername/netcrafter-templates/issues)
- 💬 [Discussions](https://github.com/yourusername/netcrafter-templates/discussions)
- 📞 [Support Guide](SUPPORT.md) - How to get help and report issues

---

<div align="center">

**[⬆ Back to Top](#netcrafter-templates-️)**

Made with ❤️ by the NetCrafter Team

</div>
