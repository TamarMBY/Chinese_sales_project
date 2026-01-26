using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using server.Data;
using server.Interfaces;
using server.Middleware;
using server.Repositories;
using server.Services;
using StoreApi.Services;
using System.Text;

try
{
    Log.Information("Starting Store API application");
    var builder = WebApplication.CreateBuilder(args);

    // 1. Serilog Configuration
    builder.Host.UseSerilog();

    // 2. Add services to the container
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();

    // 3. Swagger with JWT Support
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token in the format: Bearer {token}"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // 4. Database Context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=srv2\\pupils;DataBase=ChineseSaleDb216306829;Integrated Security=SSPI;Persist Security Info=False;TrustServerCertificate=True")
    );

    // 5. Dependency Injection (Repositories & Services)
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IDonorRepository, DonorRepository>();
    builder.Services.AddScoped<IDonorService, DonorService>();
    builder.Services.AddScoped<IGiftRepository, GiftRepository>();
    builder.Services.AddScoped<IGiftService, GiftService>();
    builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
    builder.Services.AddScoped<IPurchaseService, PurchaseService>();
    builder.Services.AddScoped<ITicketRepository, TicketRepository>();
    builder.Services.AddScoped<ITicketService, TicketService>();
    builder.Services.AddScoped<IPackageRepository, PackageRepository>();
    builder.Services.AddScoped<IPackageService, PackageService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITokenService, TokenService>();

    // 6. CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy => policy
            .WithOrigins(
            "http://localhost:4200",
            "https://localhost:58462"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

    // 7. JWT Authentication Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Log.Debug("JWT token validated for user {UserId}", userId);
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization();

    // --- Build Application ---
    var app = builder.Build();

    // 8. Configure the HTTP request pipeline (ORDER MATTERS)

    // קודם כל CORS כדי לאפשר דפדפנים
    app.UseCors("AllowAngular");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Middleware מותאם אישית (לוגים והגבלת קצב)
    app.UseRequestLogging();
    app.UseRateLimiting();

    // חובה: קודם אימות ואז הרשאות
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Store API is now running");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}