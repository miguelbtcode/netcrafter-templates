using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.Description).HasMaxLength(1000);

        builder.OwnsOne(
            p => p.Price,
            price =>
            {
                price.Property(m => m.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)");

                price.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
            }
        );

        builder.Property(p => p.Status).HasConversion<string>();

        builder.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId);

        builder.Ignore(p => p.DomainEvents);
    }
}
