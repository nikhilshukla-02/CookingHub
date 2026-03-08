using System;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnetapp.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email)
                    || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Invalid payload");
                }

                var (status, result) = await _authService.Login(model);

                if (status == 0)
                {
                    return BadRequest(result);
                }

                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid payload");
                }

                var (status, message) = await _authService.Registration(
                    model, model.UserRole);

                if (status == 0)
                {
                    return BadRequest(message);
                }

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}