using MediatR;

namespace NetCoreTemplate.Application.Users.Queries.GetUserList {
  public class GetUsersListQuery : IRequest<UsersListViewModel> {
  }
}
