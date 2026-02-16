using MechiraSinit.Data;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;



// --- הגדרת Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // לכתוב לחלון השחור
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // לכתוב לקובץ יומי בתיקיית logs
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 2. חיבור Serilog לתוך ה-Builder
    builder.Host.UseSerilog();

    // ... כאן ממשיך הקוד הרגיל שלך (AddControllers, AddDbContext וכו') ...


    // Add services to the container.

    builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MechiraSinit API", Version = "v1" });

    // הגדרת כפתור ה-Authorize (מנעול)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ⭐⭐⭐ 1. הוספת שירות ה-CORS (לפני ה-Build) ⭐⭐⭐
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // מסכים לקבל מכל כתובת (גם 4200)
              .AllowAnyMethod()  // מסכים לכל סוג פעולה (GET, POST, PUT...)
              .AllowAnyHeader(); // מסכים לכל סוגי הכותרות
    });
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⭐⭐⭐ 2. הפעלת ה-CORS (חובה שזה יהיה כאן!) ⭐⭐⭐
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "האפליקציה קרסה באופן בלתי צפוי");
}
finally
{
    Log.CloseAndFlush();
}