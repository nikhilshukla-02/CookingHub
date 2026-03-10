using Microsoft.EntityFrameworkCore;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Data;

namespace dotnetapp.Services
{
    public class CookingClassRequestService
    {
        private readonly ApplicationDbContext _context;

        public CookingClassRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Get All Cooking Class Requests with Related Details
        public async Task<IEnumerable<CookingClassRequest>> GetAllCookingClassRequests()
        {
            return await _context.CookingClassRequests
                .Include(r => r.CookingClass)
                .Include(r => r.User)
                .ToListAsync();
        }

        // 2. Get Cooking Class Requests By UserId
        public async Task<IEnumerable<CookingClassRequest>> GetCookingClassRequestsByUserId(int userId)
        {
            return await _context.CookingClassRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.CookingClass)
                .ToListAsync();
        }

        // 3. Add Cooking Class Request — UserId already set from JWT in controller
        public async Task<bool> AddCookingClassRequest(CookingClassRequest request)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists)
                throw new CookingClassException($"User with ID {request.UserId} does not exist.");

            var classExists = await _context.CookingClasses.AnyAsync(c => c.CookingClassId == request.CookingClassId);
            if (!classExists)
                throw new CookingClassException($"Cooking Class with ID {request.CookingClassId} does not exist.");

            var exists = await _context.CookingClassRequests
                .AnyAsync(r => r.CookingClassId == request.CookingClassId && r.UserId == request.UserId);
            if (exists)
                throw new CookingClassException("User already requested this cooking class");

            await _context.CookingClassRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Update Cooking Class Request — Admin can only update Status
        public async Task<bool> UpdateCookingClassRequest(int requestId, CookingClassRequest request)
        {
            var existingRequest = await _context.CookingClassRequests.FindAsync(requestId);

            if (existingRequest == null)
                return false;

            existingRequest.Status = request.Status;

            _context.CookingClassRequests.Update(existingRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Delete Cooking Class Request
        public async Task<bool> DeleteCookingClassRequest(int requestId)
        {
            var request = await _context.CookingClassRequests.FindAsync(requestId);

            if (request == null)
                return false;

            _context.CookingClassRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        // 6. Patch Cooking Class Request — User only, Status is never updated
        public async Task<bool> PatchCookingClassRequest(int requestId, int userId, CookingClassRequest patchRequest)
        {
            var existingRequest = await _context.CookingClassRequests.FindAsync(requestId);

            if (existingRequest == null)
                return false;

            if (existingRequest.UserId != userId)
                throw new CookingClassException("You are not authorized to update this request.");

            // Only these 4 fields are updated — Status is intentionally never touched

            if (patchRequest.DietaryPreferences != null)
                existingRequest.DietaryPreferences = patchRequest.DietaryPreferences;

            if (patchRequest.CookingGoals != null)
                existingRequest.CookingGoals = patchRequest.CookingGoals;

            if (patchRequest.Comments != null)
                existingRequest.Comments = patchRequest.Comments;

            _context.CookingClassRequests.Update(existingRequest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
