using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dotnetapp.Services
{
    public class ChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public ChatService(
            ApplicationDbContext context,
            IHttpClientFactory factory,
            IConfiguration config)
        {
            _context = context;
            _factory = factory;
            _config = config;
        }

        public async Task<string> GetChatReply(string userMessage, int userId = 0)
        {
            var apiKey = _config.GetSection("Groq")["ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
                return "Groq API key not configured.";

            // ── 1. ALL AVAILABLE CLASSES ──────────────────────────────
            var classes = await _context.CookingClasses.ToListAsync();

            var classContext = classes.Any()
                ? string.Join("\n", classes.Select(c =>
                    $"- ID:{c.CookingClassId} | Name:{c.ClassName} | Cuisine:{c.CuisineType} | " +
                    $"Chef:{c.ChefName} | Level:{c.SkillLevel} | Fee:{c.Fee} | " +
                    $"Duration:{c.DurationInHours}hrs | Location:{c.Location} | " +
                    $"Ingredients:{c.IngredientsProvided} | Requirements:{c.SpecialRequirements}"))
                : "No classes available currently.";

            // ── 2. USER'S APPLIED REQUESTS (if logged in) ────────────
            var userRequestsContext = "User is not logged in.";
            var userFeedbacksContext = "User is not logged in.";

            if (userId > 0)
            {
                var userRequests = await _context.CookingClassRequests
                    .Where(r => r.UserId == userId)
                    .ToListAsync();

                if (userRequests.Any())
                {
                    // Join with classes to get class names
                    var requestLines = userRequests.Select(r =>
                    {
                        var cls = classes.FirstOrDefault(c => c.CookingClassId == r.CookingClassId);
                        return $"- RequestID:{r.CookingClassRequestId} | " +
                               $"Class:{cls?.ClassName ?? "Unknown"} | " +
                               $"Status:{r.Status} | " +
                               $"Date:{r.RequestDate} | " +
                               $"DietaryPrefs:{r.DietaryPreferences} | " +
                               $"Goals:{r.CookingGoals} | " +
                               $"Comments:{r.Comments ?? "None"}";
                    });
                    userRequestsContext = string.Join("\n", requestLines);
                }
                else
                {
                    userRequestsContext = "User has not applied to any classes yet.";
                }

                // ── 3. USER'S FEEDBACKS ───────────────────────────────
                var userFeedbacks = await _context.Feedbacks
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                if (userFeedbacks.Any())
                {
                    var feedbackLines = userFeedbacks.Select(f =>
                        $"- FeedbackID:{f.FeedbackId} | " +
                        $"Text:{f.FeedbackText} | " +
                        $"Date:{f.Date}");
                    userFeedbacksContext = string.Join("\n", feedbackLines);
                }
                else
                {
                    userFeedbacksContext = "User has not submitted any feedbacks yet.";
                }
            }

            // ── 4. BUILD SYSTEM PROMPT ────────────────────────────────
            var systemPrompt = $@"You are a friendly and smart assistant for 'Cooking Hub' platform.
You have access to live data from the database. Use this data to give accurate, helpful answers.

=== ALL AVAILABLE COOKING CLASSES ===
{classContext}

=== THIS USER'S APPLIED CLASS REQUESTS ===
{userRequestsContext}

=== THIS USER'S SUBMITTED FEEDBACKS ===
{userFeedbacksContext}

You help users with:
1. Recommending classes based on skill level, cuisine, budget, goals
   - Always refer to real class names from the database above
2. Explaining request status (Pending/Approved/Rejected)
   - If user asks about their requests, show them from the data above
3. BMI Calculator: weight(kg) divided by height(m) squared
   Underweight: less than 18.5 | Normal: 18.5-24.9 | Overweight: 25-29.9 | Obese: 30+
4. Calorie Calculator using Harris-Benedict formula:
   Men: BMR = 88.36 + (13.4 x weight kg) + (4.8 x height cm) - (5.7 x age)
   Women: BMR = 447.6 + (9.2 x weight kg) + (3.1 x height cm) - (4.3 x age)
   Activity multipliers: Sedentary x1.2, Light x1.375, Moderate x1.55, Active x1.725
5. Cooking tips and nutrition advice

Rules:
- Always refer to actual data from the database sections above
- When showing requests, clearly mention class name and current status
- Keep responses short, friendly, and helpful
- Plain text only, absolutely no markdown symbols like *, #, **, etc.
- If user asks about something not in the data, say so honestly";

            // ── 5. CALL GROQ API ──────────────────────────────────────
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = userMessage  }
                },
                max_tokens = 600,
                temperature = 0.7
            };

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions", content);

            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== GROQ RESPONSE ===");
            Console.WriteLine(responseString);
            Console.WriteLine("====================");

            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.TryGetProperty("error", out var errorElement))
            {
                var errorMsg = errorElement.GetProperty("message").GetString();
                return $"AI error: {errorMsg}";
            }

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response.";
        }
    }
}