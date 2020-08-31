using MediatR;

namespace NetCoreTemplate.Application.Users.Queries.GetUserDetail {
  public class GetUserDetailQuery : IRequest<UserDetailModel> {
    public int Id { get; set; }
  }
}
