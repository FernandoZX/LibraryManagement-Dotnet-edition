using LibraryManagement.Application.Auth;
using LibraryManagement.Application.Auth.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        {
            var result = await _auth.RegisterAsync(request, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            var result = await _auth.LoginAsync(request, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }
    }
}
