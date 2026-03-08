using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using dotnetapp.Data;
using dotnetapp.Models;
using dotnetapp.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. DATABASE CONFIGURATION
// ============================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

// ============================================================
// 2. IDENTITY CONFIGURATION
// ============================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ============================================================
// 3. JWT AUTHENTICATION CONFIGURATION
//    Make sure appsettings.json contains:
//    "JWT": {
//       "ValidAudience": "...",
//       "ValidIssuer": "...",
//       "Secret": "a-strong-secret-key"
//    }
// ============================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // set true in production with HTTPS
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? string.Empty))
    };
});

// (Recommended) Authorization services
builder.Services.AddAuthorization();

// ============================================================
// 4. REGISTER SERVICES (Dependency Injection)
// ============================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CookingClassService>();
builder.Services.AddScoped<CookingClassRequestService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<ChatService>();      // ← ADD
builder.Services.AddHttpClient(); 

// ============================================================
// 5. CONTROLLERS & JSON
// ============================================================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // ← ADD THIS
});

builder.Services.AddEndpointsApiExplorer();

// ============================================================
// 6. SWAGGER (SWASHBUCKLE) WITH JWT BEARER
//    This adds the "Authorize" button and sends Authorization: Bearer <token>
// ============================================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cooking Hub API",
        Version = "v1"
    });

    // Define the BearerAuth scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste your JWT token here (without the 'Bearer ' prefix).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    // Require Bearer token for all operations (you can scope this per operation if needed)
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// ============================================================
// 7. CORS CONFIGURATION
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// ── 6. CONTROLLERS + NEWTONSOFT JSON ──────────────────────────────────────────
// AddControllers → registers all controller classes in the Controllers folder
// AddNewtonsoftJson → replaces default System.Text.Json with Newtonsoft.Json
// ReferenceLoopHandling.Ignore → prevents infinite loop crash when serializing
//   models with circular navigation properties e.g.
//   Booking → User, User → Bookings → Booking → ... (would crash without this)
// USE THIS INSTEAD
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
       
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


// ============================================================
// 8. BUILD
// ============================================================
var app = builder.Build();

// ============================================================
// 9. MIDDLEWARE PIPELINE (ORDER MATTERS!)
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Remove or comment out HTTPS redirection for HTTP testing
// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();   // ← MUST come BEFORE UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();