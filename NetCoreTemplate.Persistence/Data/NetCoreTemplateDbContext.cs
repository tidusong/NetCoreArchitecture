using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTemplate.Persistence.Data {
  public class NetCoreTemplateDbContext : DbContext {
    public NetCoreTemplateDbContext(DbContextOptions<NetCoreTemplateDbContext> options) : base(options) {}

    public DbSet<User> User { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetCoreTemplateDbContext).Assembly);
    }

    public override int SaveChanges() {
      AddAuditInfo();
      return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync() {
      AddAuditInfo();
      return await base.SaveChangesAsync();
    }

    private void AddAuditInfo() {
      var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity &&
        (x.State == EntityState.Added || x.State == EntityState.Modified));
      foreach (var entry in entries) {
        if (entry.State == EntityState.Added) {
          ((BaseEntity)entry.Entity).CreatedAtUtc = DateTime.UtcNow;
        }
        ((BaseEntity)entry.Entity).ModifiedAtUtc = DateTime.UtcNow;
      }
    }
  }
}
