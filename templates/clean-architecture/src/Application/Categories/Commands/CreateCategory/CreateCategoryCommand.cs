using Application.Common.Models;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Description) : IRequest<Result<Guid>>;
