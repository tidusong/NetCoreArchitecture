using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreTemplate.Application.Auth.Commands.Register;
using NetCoreTemplate.Application.Auth.Queries.Login;
using NetCoreTemplate.Application.Auth.Queries.RefreshToken;

namespace NetCoreTemplate.WebApi.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ApiBaseController {
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterCommand value) {
      return CreatedAtAction("register", await Mediator.Send(value));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginQuery value) {
      return Ok(await Mediator.Send(value));
    }

    [HttpPost("refreshtoken")]
    [Authorize]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenQuery value) {
      return Ok(await Mediator.Send(value));
    }
  }
}