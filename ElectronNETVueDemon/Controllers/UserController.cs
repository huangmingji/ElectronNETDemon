using System;
using System.Threading.Tasks;
using ElectronNETVueDemon.Authentication;
using ElectronNETVueDemon.DataModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ElectronNETVueDemon.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private static readonly UserData user = new();
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        public UserController(IAccessTokenGenerator accessTokenGenerator)
        {
            _accessTokenGenerator = accessTokenGenerator;
        }
        
        [HttpGet("")]
        [Authorize("admin")]
        public UserData Index()
        {
            return user;
        }

        [HttpPost("login")]
        public LoginResponseDto Login(LoginRequestDto requestDto)
        {
            if (user.Account != requestDto.UserName || user.Password != requestDto.Password)
            {
                return new LoginResponseDto(false, "invalid account password.");
            }

            var token = _accessTokenGenerator.GenerateToken(user.Id.ToString(), user.Permissions, DateTime.Now.AddHours(2));
            return new LoginResponseDto(true, "login success.", token);
        }

        [HttpPost("logout")]
        [Authorize("admin")]
        public async Task<ResponseDto> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new ResponseDto(true, "logout success");
        }
    }
}
