
using System;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace dotnetapp.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthenticationController));

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.Info($"Login attempt for email: {model?.Email}");
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email)
                    || string.IsNullOrEmpty(model.Password))
                {
                    _logger.Warn("Login failed: Invalid payload");
                    return BadRequest("Invalid payload");
                }

                var (status, result) = await _authService.Login(model);

                if (status == 0)
                {
                    _logger.Warn($"Login failed for email: {model.Email}");
                    return BadRequest(result);
                }

                _logger.Info($"Login successful for email: {model.Email}");
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                _logger.Error($"Login error: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            _logger.Info($"Register attempt for email: {model?.Email}");
            try
            {
                if (model == null)
                {
                    _logger.Warn("Register failed: Invalid payload");
                    return BadRequest("Invalid payload");
                }

                var (status, message) = await _authService.Registration(model, model.UserRole);

                if (status == 0)
                {
                    _logger.Warn($"Register failed for email: {model.Email} — {message}");
                    return BadRequest(message);
                }

                _logger.Info($"Register successful for email: {model.Email}");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.Error($"Register error: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }

        // NEW ENDPOINT
        [HttpGet("/api/verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return BadRequest("Token is required");

                var (status, message) = await _authService.VerifyEmail(token);

                if (status == 0)
                    return BadRequest(message);

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
