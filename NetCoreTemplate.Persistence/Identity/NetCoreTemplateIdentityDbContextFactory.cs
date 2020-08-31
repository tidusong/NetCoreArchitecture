using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Persistence.Infrastructure;

namespace NetCoreTemplate.Persistence.Identity {
  public class NetCoreTemplateIdentityDbContextFactory : DesignTimeDbContextFactoryBase<NetCoreTemplateIdentityDbContext> {
    protected override NetCoreTemplateIdentityDbContext CreateNewInstance(DbContextOptions<NetCoreTemplateIdentityDbContext> options) {
      return new NetCoreTemplateIdentityDbContext(options);
    }
  }
}
