using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;   // NEW

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IEmailService emailService)   // ADDED emailService parameter
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;   // INITIALIZE
        }

        public async Task<(int, string)> Registration(User model, string role)
        {
            // Check if user already exists by email
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return (0, "User already exists");
            }

            // Truncate Name to 30 characters if longer
            string truncatedName = model.Username.Length > 30
                ? model.Username.Substring(0, 30)
                : model.Username;

            // Create ApplicationUser (Identity user)
            ApplicationUser appUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = truncatedName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await userManager.CreateAsync(appUser, model.Password);

            if (!createResult.Succeeded)
            {
                return (0, "User creation failed! Please check user details and try again");
            }

            // Ensure the role exists, if not create it
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Assign role to the user
            await userManager.AddToRoleAsync(appUser, role);

            // NEW: Generate verification token
            string verificationToken = Guid.NewGuid().ToString();

            // Also save user info in the custom Users table (with verification fields)
            var dbUser = new User
            {
                Email = model.Email,
                Password = model.Password,
                Username = model.Username,
                MobileNumber = model.MobileNumber,
                UserRole = role,
                IsVerified = false,
                VerificationToken = verificationToken
            };

            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();

            // NEW: Send verification email
            await _emailService.SendVerificationEmailAsync(model.Email, verificationToken);

            return (1, "User created successfully! Please verify your email before logging in.");
        }

        public async Task<(int, string)> Login(LoginModel model)
        {
            // Find user by email
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return (0, "Invalid credentials");
            }

            // Check password
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                return (0, "Invalid password");
            }

            // ✅ Fetch the custom User to check verification and get UserId
            var customUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (customUser == null)
            {
                return (0, "User profile not found");
            }

            // NEW: Check if email is verified
            if (!customUser.IsVerified)
            {
                return (0, "Please verify your email first.");
            }

            // Get user roles
            var userRoles = await userManager.GetRolesAsync(user);

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", customUser.UserId.ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate JWT token
            string token = GenerateToken(claims);

            return (1, token);
        }

        // NEW: VerifyEmail method
        public async Task<(int, string)> VerifyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return (0, "Invalid or expired token");
            }

            user.IsVerified = true;
            user.VerificationToken = null; // token used once
            await _context.SaveChangesAsync();

            return (1, "Email verified successfully. You can now login.");
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var jwtSecret = _configuration["JWT:Secret"];
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                    authSigningKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
