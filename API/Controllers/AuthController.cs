using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using votesystembackend.API.Responses;
using votesystembackend.Application.Responses;

namespace votesystembackend.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [EnableRateLimiting("RegisterPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var res = await _auth.RegisterAsync(req);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            if (res.Success) return StatusCode(res.StatusCode, envelope);
            return StatusCode(res.StatusCode, envelope);
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var res = await _auth.LoginAsync(req);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }
    }
}
