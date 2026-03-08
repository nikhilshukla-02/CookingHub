    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using dotnetapp.Models;

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

        // 1. Get All Cooking Class Requests
            [HttpGet]
            public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetAllCookingClassRequests()
            {
                var requests = await _cookingClassRequestService.GetAllCookingClassRequests();
                return Ok(requests);
            }

        // 2. Get Cooking Class Requests By UserId
            [HttpGet("user/{userId}")]
            public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetCookingClassRequestsByUserId(int userId)
            {
                var requests = await _cookingClassRequestService.GetCookingClassRequestsByUserId(userId);
                return Ok(requests);
            }

        // 3. Add Cooking Class Request
            [HttpPost]
            public async Task<ActionResult> AddCookingClassRequest([FromBody] CookingClassRequest request)
            {
                try
                {
                    await _cookingClassRequestService.AddCookingClassRequest(request);
                    return Ok("Cooking class request added successfully");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

        // 4. Update Cooking Class Request
            [HttpPut("{requestId}")]
            public async Task<ActionResult> UpdateCookingClassRequest(int requestId, [FromBody] CookingClassRequest request)
            {
                try
                {
                    var result = await _cookingClassRequestService.UpdateCookingClassRequest(requestId, request);
                    if (!result)
                    {
                        return NotFound("Cannot find the request");
                    }

                    
                    return Ok("Cooking class request updated successfully");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

        // 5. Delete Cooking Class Request
            [HttpDelete("{requestId}")]
            public async Task<ActionResult> DeleteCookingClassRequest(int requestId)
            {
                try
                {
                    var result = await _cookingClassRequestService.DeleteCookingClassRequest(requestId);
                    if (!result)
                    {
                        return NotFound("Cannot find the request");
                    }
                    return Ok("Cooking class request deleted successfully");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
    }