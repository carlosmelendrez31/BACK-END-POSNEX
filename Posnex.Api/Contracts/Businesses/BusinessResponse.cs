using Posnex.Entities;

namespace Posnex.Api.Contracts.Businesses;

public sealed record BusinessResponse(
    Guid Id,
    string Name,
    string Type,
    string PrimaryColor,
    string SecondaryColor,
    string? LogoUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static BusinessResponse From(Business business) => new(
        business.Id,
        business.Name,
        business.Type.ToString(),
        business.PrimaryColor,
        business.SecondaryColor,
        business.LogoUrl,
        business.CreatedAt,
        business.UpdatedAt);
}
