using System.ComponentModel.DataAnnotations;

namespace Posnex.Api.Contracts.Businesses;

public sealed class CreateBusinessRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, RegularExpression("^(?i:Ecommerce|Restaurant)$")]
    public string Type { get; init; } = string.Empty;

    [Required, RegularExpression("^#[0-9a-fA-F]{6}$")]
    public string PrimaryColor { get; init; } = "#304D3D";

    [Required, RegularExpression("^#[0-9a-fA-F]{6}$")]
    public string SecondaryColor { get; init; } = "#E6C98B";

    [StringLength(2048), Url]
    public string? LogoUrl { get; init; }
}

public sealed class UpdateBusinessRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, RegularExpression("^#[0-9a-fA-F]{6}$")]
    public string PrimaryColor { get; init; } = string.Empty;

    [Required, RegularExpression("^#[0-9a-fA-F]{6}$")]
    public string SecondaryColor { get; init; } = string.Empty;

    [StringLength(2048), Url]
    public string? LogoUrl { get; init; }
}
