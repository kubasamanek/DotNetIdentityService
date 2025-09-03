using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PantryCloud.IdentityService.Core.Entities;

namespace PantryCloud.IdentityService.Core.Configurations;

internal sealed class VerifyEmailTokenConfiguration : IEntityTypeConfiguration<VerifyEmailToken>
{
    public void Configure(EntityTypeBuilder<VerifyEmailToken> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(300);
        builder.Property(u => u.Token).IsRequired();
    }
}
