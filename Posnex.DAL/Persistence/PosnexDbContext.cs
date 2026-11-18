using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Posnex.Entities;

namespace Posnex.DAL.Persistence;

public sealed class PosnexDbContext(DbContextOptions<PosnexDbContext> options)
    : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessMembership> BusinessMemberships => Set<BusinessMembership>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Business>(entity =>
        {
            entity.ToTable("businesses");
            entity.HasKey(business => business.Id);
            entity.Property(business => business.Id).HasColumnName("id");
            entity.Property(business => business.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            entity.Property(business => business.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(business => business.PrimaryColor).HasColumnName("primary_color").HasMaxLength(7).IsRequired();
            entity.Property(business => business.SecondaryColor).HasColumnName("secondary_color").HasMaxLength(7).IsRequired();
            entity.Property(business => business.LogoUrl).HasColumnName("logo_url").HasMaxLength(2048);
            entity.Property(business => business.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(business => business.UpdatedAt).HasColumnName("updated_at").IsRequired();
            entity.Property(business => business.IsDeleted).HasColumnName("is_deleted").IsRequired();
            entity.HasQueryFilter(business => !business.IsDeleted);
            entity.HasIndex(business => business.Name);
        });

        builder.Entity<BusinessMembership>(entity =>
        {
            entity.ToTable("business_memberships");
            entity.HasKey(membership => new { membership.BusinessId, membership.UserId });
            entity.Property(membership => membership.BusinessId).HasColumnName("business_id");
            entity.Property(membership => membership.UserId).HasColumnName("user_id");
            entity.Property(membership => membership.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(32);
            entity.Property(membership => membership.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(membership => membership.IsActive).HasColumnName("is_active").IsRequired();
            entity.HasQueryFilter(membership => !membership.Business.IsDeleted);
            entity.HasOne(membership => membership.Business)
                .WithMany(business => business.Memberships)
                .HasForeignKey(membership => membership.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<IdentityUser<Guid>>()
                .WithMany()
                .HasForeignKey(membership => membership.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(membership => new { membership.UserId, membership.IsActive });
        });
    }
}
