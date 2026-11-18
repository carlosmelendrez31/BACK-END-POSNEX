namespace Posnex.Entities;

public sealed class Business
{
    private Business() { }

    public Business(
        Guid id,
        string name,
        BusinessType type,
        string primaryColor,
        string secondaryColor,
        string? logoUrl,
        DateTimeOffset now)
    {
        Id = id;
        Name = name;
        Type = type;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        LogoUrl = logoUrl;
        CreatedAt = now;
        UpdatedAt = now;
        IsDeleted = false;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public BusinessType Type { get; private set; }
    public string PrimaryColor { get; private set; } = "#304D3D";
    public string SecondaryColor { get; private set; } = "#E6C98B";
    public string? LogoUrl { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public ICollection<BusinessMembership> Memberships { get; } = new List<BusinessMembership>();

    public void UpdateBranding(
        string name,
        string primaryColor,
        string secondaryColor,
        string? logoUrl,
        DateTimeOffset now)
    {
        Name = name;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        LogoUrl = logoUrl;
        UpdatedAt = now;
    }

    public void SoftDelete(DateTimeOffset now)
    {
        IsDeleted = true;
        UpdatedAt = now;
    }
}
