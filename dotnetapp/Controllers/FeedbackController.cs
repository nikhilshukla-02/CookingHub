using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
 
namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // optional but helpful
    public class FeedbackController : ControllerBase
    {
        public FeedbackService _feedbackService;
 
        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
 
        // 1. Get all feedbacks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacks();
                return Ok(feedbacks); // already JSON (list of objects)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
 
        // 2. Get feedbacks by userId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUserId(int userId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByUserId(userId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
 
        // 3. Add a new feedback
        [HttpPost]
        public async Task<ActionResult> AddFeedback([FromBody] Feedback feedback)
        {
            try
            {
                await _feedbackService.AddFeedback(feedback);
                return Ok(new { success = true, message = "Feedback added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
 
        // 4. Delete feedback by feedbackId
        [HttpDelete("{feedbackId}")]
        public async Task<ActionResult> DeleteFeedback(int feedbackId)
        {
            try
            {
                bool deleted = await _feedbackService.DeleteFeedback(feedbackId);
 
                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Cannot find any feedback" });
                }
 
                return Ok(new { success = true, message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
 