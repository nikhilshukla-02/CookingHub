using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Models;
using dotnetapp.Exceptions;

namespace dotnetapp.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookingClassRequestController : ControllerBase
    {
        private readonly CookingClassRequestService _cookingClassRequestService;

        public CookingClassRequestController(CookingClassRequestService cookingClassRequestService)
        {
            _cookingClassRequestService = cookingClassRequestService;
        }

        private string? GetRoleFromToken()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userIdClaim == null) return null;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetAllCookingClassRequests()
        {
            var requests = await _cookingClassRequestService.GetAllCookingClassRequests();
            return Ok(requests);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetCookingClassRequestsByUserId(int userId)
        {
            var requests = await _cookingClassRequestService.GetCookingClassRequestsByUserId(userId);
            return Ok(requests);
        }

        // POST — User only, UserId from JWT
        [HttpPost]
        public async Task<ActionResult> AddCookingClassRequest([FromBody] CookingClassRequest request)
        {
            var role = GetRoleFromToken();
            if (role != "User")
                return StatusCode(403, new { message = "Only users can add a cooking class request." });

            var userId = GetUserIdFromToken();
            if (userId == null)
                return StatusCode(401, new { message = "Invalid token. User ID not found." });

            request.UserId = userId.Value;

            try
            {
                await _cookingClassRequestService.AddCookingClassRequest(request);
                return Ok(new { message = "Cooking class request added successfully" });
            }
            catch (CookingClassException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT — Admin only, can only update Status
        [HttpPut("{requestId}")]
        public async Task<ActionResult> UpdateCookingClassRequest(int requestId, [FromBody] CookingClassRequest request)
        {
            var role = GetRoleFromToken();
            if (role != "Admin")
                return StatusCode(403, new { message = "Only admins can update a cooking class request." });

            if (request.DietaryPreferences != null || request.CookingGoals != null || request.Comments != null)
                return StatusCode(403, new { message = "Admin can only update Status. Other fields are not allowed." });

            try
            {
                var result = await _cookingClassRequestService.UpdateCookingClassRequest(requestId, request);
                if (!result)
                    return NotFound(new { message = "Cannot find the request" });

                return Ok(new { message = "Cooking class request updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{requestId}")]
        public async Task<ActionResult> DeleteCookingClassRequest(int requestId)
        {
            try
            {
                var result = await _cookingClassRequestService.DeleteCookingClassRequest(requestId);
                if (!result)
                    return NotFound(new { message = "Cannot find the request" });

                return Ok(new { message = "Cooking class request deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH — User only, Status never updated
        [HttpPatch("{requestId}")]
        public async Task<ActionResult> PatchCookingClassRequest(int requestId, [FromBody] CookingClassRequest request)
        {
            var role = GetRoleFromToken();
            if (role != "User")
                return StatusCode(403, new { message = "Only users can patch a cooking class request." });

            var userId = GetUserIdFromToken();
            if (userId == null)
                return StatusCode(401, new { message = "Invalid token. User ID not found." });

            try
            {
                var result = await _cookingClassRequestService.PatchCookingClassRequest(requestId, userId.Value, request);
                if (!result)
                    return NotFound(new { message = "Cannot find the request" });

                return Ok(new { message = "Cooking class request patched successfully" });
            }
            catch (CookingClassException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
