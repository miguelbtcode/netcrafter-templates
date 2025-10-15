using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public ProductStatus Status { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Product() { } // EF Core

    public Product(string name, string description, Money price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty");

        Name = name;
        Description = description;
        Price = price;
        Status = ProductStatus.Draft;
        CategoryId = categoryId;

        AddDomainEvent(new ProductCreatedEvent(Id, name));
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be greater than zero");

        Price = newPrice;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == ProductStatus.Published)
            throw new DomainException("Product is already published");

        Status = ProductStatus.Published;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = ProductStatus.Archived;
        LastModifiedAt = DateTime.UtcNow;
    }
}
