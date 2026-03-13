using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using log4net;

namespace dotnetapp.Services
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CookingClassController : ControllerBase
    {
        private readonly CookingClassService _cookingClassService;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CookingClassController));

        public CookingClassController(CookingClassService cookingClassService)
        {
            _cookingClassService = cookingClassService;
        }

        private string? GetRoleFromToken()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.ToLower();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CookingClass>>> GetAllCookingClasses()
        {
            _logger.Info("GetAllCookingClasses called");
            try
            {
                var classes = await _cookingClassService.GetAllCookingClasses();
                _logger.Info($"GetAllCookingClasses returned {classes.Count()} records");
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAllCookingClasses error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{classId}")]
        public async Task<ActionResult<CookingClass>> GetCookingClassById(int classId)
        {
            _logger.Info($"GetCookingClassById called for classId: {classId}");
            try
            {
                var cookingClass = await _cookingClassService.GetCookingClassById(classId);
                if (cookingClass == null)
                {
                    _logger.Warn($"GetCookingClassById: not found for classId: {classId}");
                    return NotFound(new { message = "Cannot find any cooking class" });
                }
                return Ok(cookingClass);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetCookingClassById error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddCookingClass([FromBody] CookingClass cooking)
        {
            var role = GetRoleFromToken();
            _logger.Info($"AddCookingClass called with role: {role}");
            if (role != "admin")
            {
                _logger.Warn($"AddCookingClass forbidden for role: {role}");
                return StatusCode(403, new { message = "Only admins can add a cooking class." });
            }

            try
            {
                var result = await _cookingClassService.AddCookingClass(cooking);
                if (result)
                {
                    _logger.Info($"Cooking class added successfully: {cooking.ClassName}");
                    return Ok(new { message = "Cooking class added successfully" });
                }
                return StatusCode(500, new { message = "Failed to add cooking class" });
            }
            catch (CookingClassException ex)
            {
                _logger.Warn($"AddCookingClass business error: {ex.Message}");
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"AddCookingClass error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{classId}")]
        public async Task<ActionResult> UpdateCookingClass(int classId, [FromBody] CookingClass cooking)
        {
            var role = GetRoleFromToken();
            _logger.Info($"UpdateCookingClass called for classId: {classId} with role: {role}");
            if (role != "admin")
            {
                _logger.Warn($"UpdateCookingClass forbidden for role: {role}");
                return StatusCode(403, new { message = "Only admins can update a cooking class." });
            }

            try
            {
                var result = await _cookingClassService.UpdateCookingClass(classId, cooking);
                if (!result)
                {
                    _logger.Warn($"UpdateCookingClass: not found for classId: {classId}");
                    return NotFound(new { message = "Cannot find any cooking class" });
                }
                _logger.Info($"Cooking class updated successfully for classId: {classId}");
                return Ok(new { message = "Cooking class updated successfully" });
            }
            catch (CookingClassException ex)
            {
                _logger.Warn($"UpdateCookingClass business error: {ex.Message}");
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateCookingClass error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{classId}")]
        public async Task<ActionResult> DeleteCookingClass(int classId)
        {
            var role = GetRoleFromToken();
            _logger.Info($"DeleteCookingClass called for classId: {classId} with role: {role}");
            if (role != "admin")
            {
                _logger.Warn($"DeleteCookingClass forbidden for role: {role}");
                return StatusCode(403, new { message = "Only admins can delete a cooking class." });
            }

            try
            {
                var result = await _cookingClassService.DeleteCookingClass(classId);
                if (!result)
                {
                    _logger.Warn($"DeleteCookingClass: not found for classId: {classId}");
                    return NotFound(new { message = "Cannot find any cooking class" });
                }
                _logger.Info($"Cooking class deleted successfully for classId: {classId}");
                return Ok(new { message = "Cooking class deleted successfully" });
            }
            catch (CookingClassException ex)
            {
                _logger.Warn($"DeleteCookingClass business error: {ex.Message}");
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteCookingClass error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
