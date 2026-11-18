using Posnex.Entities;

namespace Posnex.BLL.Businesses;

public sealed record CreateBusinessCommand(
    string Name,
    BusinessType Type,
    string PrimaryColor,
    string SecondaryColor,
    string? LogoUrl);

public sealed record UpdateBusinessCommand(
    string Name,
    string PrimaryColor,
    string SecondaryColor,
    string? LogoUrl);

public interface IBusinessRepository
{
    Task<IReadOnlyList<Business>> ListForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Business?> GetForUserAsync(Guid businessId, Guid userId, CancellationToken cancellationToken);
    Task AddForOwnerAsync(Business business, Guid userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IBusinessService
{
    Task<IReadOnlyList<Business>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<Business?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Business> CreateAsync(CreateBusinessCommand command, Guid userId, CancellationToken cancellationToken);
    Task<Business?> UpdateAsync(Guid id, UpdateBusinessCommand command, Guid userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}

public sealed class BusinessValidationException(string message) : Exception(message);
