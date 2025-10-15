using Application.Common.Models;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
