using MediatR;
using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Persistence.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Users.Queries.GetUserList {
  public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, UsersListViewModel> {
    private NetCoreTemplateDbContext _context;

    public GetUsersListQueryHandler(NetCoreTemplateDbContext context) {
      _context = context;
    }

    public async Task<UsersListViewModel> Handle(GetUsersListQuery request, CancellationToken cancellationToken) {
      var vm = new UsersListViewModel {
        Users = await _context.User.Select(c =>
            new UserLookupModel {
              Id = c.Id,
              FirstName = c.FirstName
            }).ToListAsync(cancellationToken)
      };

      return vm;
    }
  }
}
