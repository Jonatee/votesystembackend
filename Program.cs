using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using votesystembackend.Application.Interfaces;
using votesystembackend.Application.Services;
using votesystembackend.Infrastructure.Persistence;
using votesystembackend.Infrastructure.Repositories;
using votesystembackend.Infrastructure.Services;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Hangfire;
using Hangfire.SqlServer;
using votesystembackend.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration (read from appsettings)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Hangfire configuration ---
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=VoteSystemDb;Trusted_Connection=True;";

builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(conn, new SqlServerStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(15),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(1),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Rate limiting configuration
builder.Services.AddRateLimiter(options =>
{
    // Custom JSON response for rejections
    options.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var task = context.HttpContext.Response.WriteAsync(
            "{\"success\":false,\"error\":\"Too many attempts. Slow down and try again later.\"}",
            cancellationToken: token);
        return new ValueTask(task);
    };

    // Global token bucket limiter per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown";
        return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 50,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 50,
            AutoReplenishment = true
        });
    });

    // Login limiter: strict (per IP)
    options.AddPolicy("LoginPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown_login";
        return RateLimitPartition.GetSlidingWindowLimiter(ip, _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(10),
            SegmentsPerWindow = 2,
            QueueLimit = 0
        });
    });

    // Register limiter: medium (per IP)
    options.AddPolicy("RegisterPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown_register";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromDays(365*100),
            QueueLimit = 0
        });
    });
});

// DbContext - SQL Server
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVoteService, VoteService>();

// Jwt provider
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register Hangfire job that will be activated by DI
builder.Services.AddScoped<ExpiredSessionJob>();

// Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default_secret_key_please_change");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}
else
{
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Use rate limiter BEFORE authentication to stop abusive requests early
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Schedule recurring job: close expired sessions every 5 minutes
RecurringJob.AddOrUpdate<ExpiredSessionJob>(
    "close-expired-sessions",
    job => job.CloseExpiredSessionsAsync(),
    Cron.MinuteInterval(5));

app.Run();
