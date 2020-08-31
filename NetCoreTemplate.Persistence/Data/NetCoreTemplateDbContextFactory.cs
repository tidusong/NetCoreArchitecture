using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Persistence.Infrastructure;

namespace NetCoreTemplate.Persistence.Data {
  public class NetCoreTemplateDbContextFactory : DesignTimeDbContextFactoryBase<NetCoreTemplateDbContext> {
    protected override NetCoreTemplateDbContext CreateNewInstance(DbContextOptions<NetCoreTemplateDbContext> options) {
      return new NetCoreTemplateDbContext(options);
    }
  }
}
