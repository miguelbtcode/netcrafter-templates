using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Presentation.E2ETests;

public class ProductWorkflowTests : IClassFixture<E2ETestWebAppFactory>
{
    private readonly HttpClient _client;

    public ProductWorkflowTests(E2ETestWebAppFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteProductWorkflow_Should_Succeed()
    {
        // Arrange - Create a category first
        var createCategoryRequest = new
        {
            name = "Electronics E2E",
            description = "Electronics category for E2E testing",
        };

        var categoryResponse = await _client.PostAsJsonAsync(
            "/api/categories",
            createCategoryRequest
        );
        categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();
        categoryId.Should().NotBeEmpty();

        // Act 1: Create a product
        var createProductRequest = new
        {
            name = "Test Product E2E",
            description = "This is an end-to-end test product",
            price = 99.99m,
            currency = "USD",
            categoryId,
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", createProductRequest);

        // Assert 1: Product creation
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProductId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createdProductId.Should().NotBeEmpty();

        // Act 2: Retrieve the created product
        var getResponse = await _client.GetAsync($"/api/products/{createdProductId}");

        // Assert 2: Product retrieval
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        retrievedProduct.Should().NotBeNull();
        retrievedProduct!.Name.Should().Be("Test Product E2E");
        retrievedProduct.Price.Should().Be(99.99m);
        retrievedProduct.Currency.Should().Be("USD");

        // This demonstrates a complete E2E workflow
    }

    [Fact]
    public async Task HealthCheck_Should_ReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}

// DTO for deserialization
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
