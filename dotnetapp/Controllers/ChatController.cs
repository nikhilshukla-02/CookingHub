using System;
using System.Threading.Tasks;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ChatController));

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult> Chat([FromBody] ChatRequest request)
        {
            _logger.Info("Chat endpoint called");
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    _logger.Warn("Chat failed: Message is empty");
                    return BadRequest(new { success = false, message = "Message cannot be empty" });
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                int userId = 0;
                int.TryParse(userIdClaim, out userId);
                _logger.Info($"Chat request from userId: {userId}");

                var reply = await _chatService.GetChatReply(request.Message, userId);
                _logger.Info($"Chat reply generated for userId: {userId}");
                return Ok(new { success = true, reply });
            }
            catch (Exception ex)
            {
                _logger.Error($"Chat error: {ex.Message}", ex);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}
