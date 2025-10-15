using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Products.DTOs;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == request.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price.Amount,
                    Currency = p.Price.Currency,
                    Status = p.Status.ToString(),
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result<ProductDto>.Failure($"Product with ID {request.Id} not found");
            }

            return Result<ProductDto>.Success(product);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure(ex.Message);
        }
    }
}
