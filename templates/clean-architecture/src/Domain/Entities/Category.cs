using Domain.Common;

namespace Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Category() { } // EF Core

    public Category(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        LastModifiedAt = DateTime.UtcNow;
    }
}
