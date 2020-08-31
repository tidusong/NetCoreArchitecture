using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace NetCoreTemplate.Common.Auth {
  public class JwtIssuerOptions {
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Audience { get; set; }
    public DateTime Expiration => IssueAt.Add(ValidFor);
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssueAt => DateTime.UtcNow;
    public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(120);

    public Func<Task<string>> JtiGenerator =>
      () => Task.FromResult(Guid.NewGuid().ToString());

    public SigningCredentials SigningCredentials { get; set; }
  }
}
