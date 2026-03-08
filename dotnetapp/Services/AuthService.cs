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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
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

            // Also save user info in the custom Users table
            var dbUser = new User
            {
                Email = model.Email,
                Password = model.Password,
                Username = model.Username,
                MobileNumber = model.MobileNumber,
                UserRole = role
            };

            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();

            return (1, "User created successfully!");
        }

public async Task<(int, string)> Login(LoginModel model)
{
    // Find user by email
    var user = await userManager.FindByEmailAsync(model.Email);

    if (user == null)
    {
        return (0, "Invalid email");
    }

    // Check password
    if (!await userManager.CheckPasswordAsync(user, model.Password))
    {
        return (0, "Invalid password");
    }

    // Get user roles
    var userRoles = await userManager.GetRolesAsync(user);

    // ✅ Fetch the custom User to get the integer UserId
    var customUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
    if (customUser == null)
    {
        return (0, "User profile not found");
    }

    // Build claims
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("UserId", customUser.UserId.ToString())   // ✅ ADD THIS
    };

    foreach (var role in userRoles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    // Generate JWT token
    string token = GenerateToken(claims);

    return (1, token);
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