using Application.Common.Models;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId
) : IRequest<Result<Guid>>;
