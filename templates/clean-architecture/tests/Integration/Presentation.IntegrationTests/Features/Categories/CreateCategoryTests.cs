using System.Net;
using System.Net.Http.Json;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Presentation.IntegrationTests.Infrastructure;

namespace Presentation.IntegrationTests.Features.Categories;

public class CreateCategoryTests : BaseIntegrationTest
{
    public CreateCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            name = "Electronics",
            description = "Electronic devices and accessories",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var categoryId = await response.Content.ReadFromJsonAsync<Guid>();
        categoryId.Should().NotBeEmpty();

        var categoryInDb = await DbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

        categoryInDb.Should().NotBeNull();
        categoryInDb!.Name.Should().Be("Electronics");
        categoryInDb.Description.Should().Be("Electronic devices and accessories");
    }

    [Fact]
    public async Task CreateCategory_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new { name = "", description = "Test description" };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategory_MultipleTimes_ShouldCreateMultipleCategories()
    {
        // Arrange
        var request1 = new { name = "Category 1", description = "Description 1" };
        var request2 = new { name = "Category 2", description = "Description 2" };

        // Act
        await HttpClient.PostAsJsonAsync("/api/categories", request1);
        await HttpClient.PostAsJsonAsync("/api/categories", request2);

        // Assert
        var categoriesCount = await DbContext.Categories.CountAsync();
        categoriesCount.Should().Be(2);
    }
}
