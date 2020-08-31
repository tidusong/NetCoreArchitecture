using System.Collections.Generic;

namespace NetCoreTemplate.Application.Users.Queries.GetUserList {
  public class UsersListViewModel {
    public IList<UserLookupModel> Users { get; set; }
  }
}
