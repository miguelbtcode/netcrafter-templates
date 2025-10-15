namespace Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }

    protected AuditableEntity()
        : base()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
