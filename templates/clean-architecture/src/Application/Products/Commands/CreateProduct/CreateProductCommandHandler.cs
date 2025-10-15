using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var money = new Money(request.Price, request.Currency);

            var product = new Product(request.Name, request.Description, money, request.CategoryId);

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
