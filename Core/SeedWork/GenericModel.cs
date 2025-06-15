namespace Core.SeedWork;

public abstract class GenericModel
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    
    public void SetId(Guid id) => Id = id;

    public void SetCreatedAt() => CreatedAt = DateTime.UtcNow;
    
    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;

    public void SetDeletedAt() => DeletedAt = DateTime.UtcNow;
}