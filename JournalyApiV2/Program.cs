using System.Security.Cryptography.X509Certificates;
using System.Text;
using JournalyApiV2.Data;
using JournalyApiV2.Extensions;
using JournalyApiV2.Models;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options => // System.Text.Json will not deserialize times into TimeOnly correctly for reaason
{
    options.SerializerSettings.Converters.Add(new TimeOnlyConverter()); // Neither will newtonsoft appearantly
});

builder.Services.AddIdentity<JournalyUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = 
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    })
    .AddEntityFrameworkStores<JournalyDbContext>();


// Configure cross-origin resource sharing
//https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
    if (origins != null && origins.Length > 0)
    {
        options.AddPolicy(name: "CorsPolicy", policy =>
        {
            policy.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    }
    else
    {
        options.AddPolicy(name: "CorsPolicy", policy =>
        {
        });
    }
});

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

            ValidIssuer = builder.Configuration["Identity:Issuer"],
            ValidAudience = builder.Configuration["Identity:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Identity:Key"]))
        };
    });

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<HttpResponseExceptionFilter>();
        options.InputFormatters.Insert(options.InputFormatters.Count, new TextPlainInputFormatter());
    }
);
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IJournalDbService, JournalDbService>();
builder.Services.AddScoped<IResourceAccessHelper, ResourceAccessHelper>();
builder.Services.AddScoped<ISyncDbService, SyncDbService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedService, MedService>();
builder.Services.AddScoped<IMedDbService, MedDbService>();
builder.Services.AddScoped<IAuthDbService, AuthDbService>();
builder.Services.AddDbContext<JournalyDbContext>(); // Do not use this. This API uses concurrency a ton and this will cause race conditions
builder.Services.AddTransient<IDbFactory, DbFactory>(); // Use this instead 

var app = builder.Build();

 
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();