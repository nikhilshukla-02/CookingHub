using Microsoft.EntityFrameworkCore;
using dotnetapp.Models; 
using dotnetapp.Data;   

namespace dotnetapp.Services
{
    public class FeedbackService
    {
        private  ApplicationDbContext db;

        public FeedbackService(ApplicationDbContext db1)
        {
            db = db1;
        }

        // 1. Retrieves all feedbacks from the database
        public async Task<IEnumerable<Feedback>> GetAllFeedbacks()
        {
            return await db.Feedbacks.ToListAsync();
        }

        // 2. Retrieves all feedbacks associated with a specific userId
        public async Task<IEnumerable<Feedback>> GetFeedbacksByUserId(int userId)
        {
            return await db.Feedbacks
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        // 3. Adds new feedback to the database
        public async Task<bool> AddFeedback(Feedback feedback)
        {
            db.Feedbacks.Add(feedback);
            var result = await db.SaveChangesAsync();
            return result > 0;
        }

        // 4. Deletes feedback by ID
        public async Task<bool> DeleteFeedback(int feedbackId)
        {
            // a. Retrieve existing feedback
            var feedback = await db.Feedbacks.FindAsync(feedbackId);

            // b. If no feedback is found, return false
            if (feedback == null)
            {
                return false;
            }

            // c. Delete the feedback
            db.Feedbacks.Remove(feedback);

            // d. Save changes asynchronously
            var result = await db.SaveChangesAsync();

            // e. Return true for successful delete
            return result > 0;
        }
    }
}
