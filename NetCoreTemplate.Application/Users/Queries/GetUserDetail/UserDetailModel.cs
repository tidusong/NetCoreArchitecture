using NetCoreTemplate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetCoreTemplate.Application.Users.Queries.GetUserDetail {
  public class UserDetailModel {
    public int Id { get; set; }
    public string Email { get; set; }
    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
    public string LastIpAddress { get; set; }
    public DateTime CreateAtUtc { get; set; }
    public DateTime? LastLoginDateUtc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Mobile { get; set; }

    public static Expression<Func<User, UserDetailModel>> Projection {
      get {
        return user => new UserDetailModel {
          Id = user.Id,
          Email = user.Email,
          Active = user.Active,
          Deleted = user.Deleted,
          LastIpAddress = user.LastIpAddress,
          CreateAtUtc = user.CreatedAtUtc,
          LastLoginDateUtc = user.LastLoginDateUtc,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Mobile = user.Mobile
        };
      }
    }

    public static UserDetailModel Create(User user) {
      return Projection.Compile().Invoke(user);
    }
  }
}
