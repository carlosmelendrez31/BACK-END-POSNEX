using System.Text.RegularExpressions;
using Posnex.Entities;

namespace Posnex.BLL.Businesses;

public sealed class BusinessService(IBusinessRepository repository) : IBusinessService
{
    private static readonly Regex HexColor = new("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    public Task<IReadOnlyList<Business>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        repository.ListForUserAsync(userId, cancellationToken);

    public Task<Business?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        repository.GetForUserAsync(id, userId, cancellationToken);

    public async Task<Business> CreateAsync(CreateBusinessCommand command, Guid userId, CancellationToken cancellationToken)
    {
        Validate(command.Name, command.PrimaryColor, command.SecondaryColor, command.LogoUrl);
        if (!Enum.IsDefined(command.Type))
            throw new BusinessValidationException("El tipo de negocio no es válido.");

        var now = DateTimeOffset.UtcNow;
        var business = new Business(
            Guid.NewGuid(), command.Name.Trim(), command.Type,
            command.PrimaryColor, command.SecondaryColor, command.LogoUrl, now);

        await repository.AddForOwnerAsync(business, userId, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return business;
    }

    public async Task<Business?> UpdateAsync(Guid id, UpdateBusinessCommand command, Guid userId, CancellationToken cancellationToken)
    {
        Validate(command.Name, command.PrimaryColor, command.SecondaryColor, command.LogoUrl);
        var business = await repository.GetForUserAsync(id, userId, cancellationToken);
        if (business is null) return null;

        business.UpdateBranding(command.Name.Trim(), command.PrimaryColor, command.SecondaryColor, command.LogoUrl, DateTimeOffset.UtcNow);
        await repository.SaveChangesAsync(cancellationToken);
        return business;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var business = await repository.GetForUserAsync(id, userId, cancellationToken);
        if (business is null) return false;

        business.SoftDelete(DateTimeOffset.UtcNow);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(string name, string primaryColor, string secondaryColor, string? logoUrl)
    {
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length is < 2 or > 120)
            throw new BusinessValidationException("El nombre es obligatorio y debe tener máximo 120 caracteres.");
        if (!HexColor.IsMatch(primaryColor) || !HexColor.IsMatch(secondaryColor))
            throw new BusinessValidationException("Los colores deben usar el formato hexadecimal #RRGGBB.");
        if (logoUrl is not null && (!Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
            throw new BusinessValidationException("El logo debe ser una URL HTTPS válida.");
    }
}
