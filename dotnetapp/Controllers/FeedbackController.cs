using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FeedbackController : ControllerBase
    {
        public FeedbackService _feedbackService;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(FeedbackController));

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        private string? GetRoleFromToken()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.ToLower();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks()
        {
            _logger.Info("GetAllFeedbacks called");
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacks();
                _logger.Info($"GetAllFeedbacks returned {feedbacks.Count()} records");
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAllFeedbacks error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUserId(int userId)
        {
            _logger.Info($"GetFeedbacksByUserId called for userId: {userId}");
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByUserId(userId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetFeedbacksByUserId error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddFeedback([FromBody] Feedback feedback)
        {
            var role = GetRoleFromToken();
            _logger.Info($"AddFeedback called with role: {role}");
            if (role != "user")
            {
                _logger.Warn($"AddFeedback forbidden for role: {role}");
                return StatusCode(403, new { message = "Only users can add feedback." });
            }

            try
            {
                await _feedbackService.AddFeedback(feedback);
                _logger.Info("Feedback added successfully");
                return Ok(new { message = "Feedback added successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error($"AddFeedback error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{feedbackId}")]
        public async Task<ActionResult> DeleteFeedback(int feedbackId)
        {
            var role = GetRoleFromToken();
            _logger.Info($"DeleteFeedback called for feedbackId: {feedbackId} with role: {role}");
            if (role != "user")
            {
                _logger.Warn($"DeleteFeedback forbidden for role: {role}");
                return StatusCode(403, new { message = "Only users can delete feedback." });
            }

            try
            {
                bool deleted = await _feedbackService.DeleteFeedback(feedbackId);
                if (!deleted)
                {
                    _logger.Warn($"DeleteFeedback: not found for feedbackId: {feedbackId}");
                    return NotFound(new { message = "Cannot find any feedback" });
                }
                _logger.Info($"Feedback deleted successfully for feedbackId: {feedbackId}");
                return Ok(new { message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteFeedback error: {ex.Message}", ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
