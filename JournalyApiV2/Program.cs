using System.Security.Cryptography.X509Certificates;
using System.Text;
using JournalyApiV2.Data;
using JournalyApiV2.Extensions;
using JournalyApiV2.Models;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options => // System.Text.Json will not deserialize times into TimeOnly correctly for reaason
{
    options.SerializerSettings.Converters.Add(new TimeOnlyConverter()); // Neither will newtonsoft appearantly
});


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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.Requirements.Add(new EmailConfirmedRequirement()));
});

builder.Services.AddIdentity<JournalyUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = 
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        // Disable password requirements since the password will be pre-hashed anyway
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<JournalyDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<JournalyUser>>(TokenOptions.DefaultProvider);;

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = "CustomScheme";
    options.DefaultChallengeScheme = "CustomScheme";
}).AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("CustomScheme", options => {});



builder.Services.AddControllers(options =>
    {
        options.Filters.Add<HttpResponseExceptionFilter>();
        options.InputFormatters.Insert(options.InputFormatters.Count, new TextPlainInputFormatter());
    }
);
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IJournalDbService, JournalDbService>();
builder.Services.AddScoped<IResourceAccessHelper, ResourceAccessHelper>();
builder.Services.AddScoped<ISyncDbService, SyncDbService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedService, MedService>();
builder.Services.AddScoped<ICryptoDbService, CryptoDbService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IMedDbService, MedDbService>();
builder.Services.AddScoped<IAuthDbService, AuthDbService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountResetService, AccountResetService>();
builder.Services.AddDbContext<JournalyDbContext>(); // Do not use this. This API uses concurrency a ton and this will cause race conditions
builder.Services.AddTransient<IDbFactory, DbFactory>(); // Use this instead 
builder.Services.AddScoped<IAuthorizationHandler, EmailConfirmedHandler>();

var app = builder.Build();

app.Services.GetService<IDbFactory>().Journaly().Database.Migrate();
Console.WriteLine("Updated database");

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();