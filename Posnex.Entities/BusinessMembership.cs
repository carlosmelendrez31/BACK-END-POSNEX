namespace Posnex.Entities;

public enum BusinessRole
{
    Owner = 1,
    Administrator = 2,
    Staff = 3
}

public sealed class BusinessMembership
{
    private BusinessMembership() { }

    public BusinessMembership(Guid businessId, Guid userId, BusinessRole role, DateTimeOffset now)
    {
        BusinessId = businessId;
        UserId = userId;
        Role = role;
        CreatedAt = now;
        IsActive = true;
    }

    public Guid BusinessId { get; private set; }
    public Guid UserId { get; private set; }
    public BusinessRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public Business Business { get; private set; } = null!;
}
