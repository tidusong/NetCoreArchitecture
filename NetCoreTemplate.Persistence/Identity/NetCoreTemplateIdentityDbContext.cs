using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Domain.Entities;

namespace NetCoreTemplate.Persistence.Identity {
  public class NetCoreTemplateIdentityDbContext : IdentityDbContext<AppUser> {
    public NetCoreTemplateIdentityDbContext(DbContextOptions<NetCoreTemplateIdentityDbContext> options) : base(options) { }
  }
}
