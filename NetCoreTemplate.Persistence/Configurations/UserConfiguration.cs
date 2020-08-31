using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreTemplate.Domain.Entities;

namespace NetCoreTemplate.Persistence.Configurations {
  public class UserConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
      builder.HasKey(e => e.Id);

      builder.Property(e => e.Id)
        .ValueGeneratedOnAdd();

      builder.Property(e => e.FirstName).HasMaxLength(50);
      builder.Property(e => e.LastName).HasMaxLength(50);
      var navigation = builder.Metadata.FindNavigation(nameof(User.RefreshTokens));
      navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
      builder.HasMany(e => e.RefreshTokens)
        .WithOne();

      builder.Ignore(b => b.PasswordHash);
    }
  }
}
