using Microsoft.EntityFrameworkCore;
using Posnex.BLL.Businesses;
using Posnex.DAL.Persistence;
using Posnex.Entities;

namespace Posnex.DAL.Businesses;

public sealed class BusinessRepository(PosnexDbContext dbContext) : IBusinessRepository
{
    public async Task<IReadOnlyList<Business>> ListForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.Businesses
            .AsNoTracking()
            .Where(business => business.Memberships.Any(membership =>
                membership.UserId == userId && membership.IsActive && membership.Role == BusinessRole.Owner))
            .OrderByDescending(business => business.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Business?> GetForUserAsync(Guid businessId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.Businesses.FirstOrDefaultAsync(business =>
            business.Id == businessId && business.Memberships.Any(membership =>
                membership.UserId == userId && membership.IsActive && membership.Role == BusinessRole.Owner),
            cancellationToken);

    public async Task AddForOwnerAsync(Business business, Guid userId, CancellationToken cancellationToken)
    {
        await dbContext.Businesses.AddAsync(business, cancellationToken);
        await dbContext.BusinessMemberships.AddAsync(
            new BusinessMembership(business.Id, userId, BusinessRole.Owner, DateTimeOffset.UtcNow),
            cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
