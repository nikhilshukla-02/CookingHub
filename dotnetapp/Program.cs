using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using dotnetapp.Data;
using dotnetapp.Models;
using dotnetapp.Services;
using log4net;
using log4net.Config;

// ============================================================
// LOG4NET INITIALIZATION
// ============================================================
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// 1. DATABASE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

// 2. IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. JWT AUTHENTICATION
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateIssuerSigningKey = true,
        ValidAudience            = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer              = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? string.Empty))
    };
});

builder.Services.AddAuthorization();

// 4. SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CookingClassService>();
builder.Services.AddScoped<CookingClassRequestService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient();
// builder.Configuration.AddUserSecrets<Program>();

// 5. CONTROLLERS — single registration with correct JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
       
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

// 6. SWAGGER WITH JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cooking Hub API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Description = "Enter JWT token (without 'Bearer ' prefix)",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        Reference   = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id   = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// 7. CORS — no trailing slash!
const string CorsPolicyName = "Frontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(
            "https://8081-aeecccebfeecdabeebedccecabfaedfdcf.premiumproject.examly.io"
              
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 8. BUILD
var app = builder.Build();

// 9. MIDDLEWARE PIPELINE (order is critical)
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(CorsPolicyName);   // after routing, before auth

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
