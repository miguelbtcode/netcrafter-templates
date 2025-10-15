namespace Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string ProductName);
