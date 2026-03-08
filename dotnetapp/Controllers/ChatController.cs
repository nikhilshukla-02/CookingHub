using System;
using System.Threading.Tasks;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest(new { success = false, message = "Message cannot be empty" });

                // Extract userId from JWT token claim
                var userIdClaim = User.FindFirst("UserId")?.Value;
                int userId = 0;
                int.TryParse(userIdClaim, out userId);

                var reply = await _chatService.GetChatReply(request.Message, userId);
                return Ok(new { success = true, reply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}