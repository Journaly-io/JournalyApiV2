using System.Security.Cryptography.X509Certificates;
using JournalyApiV2.Data;
using JournalyApiV2.Pipeline;
using JournalyApiV2.Services.BLL;
using JournalyApiV2.Services.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers();

// Import JWT certificate
var cert = new X509Certificate2(
    Convert.FromBase64String(builder.Configuration.GetValue<string>("IdentityStore:IssuerCert")));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration.GetValue<string>("IdentityStore:Authority"),
            IssuerSigningKey = new X509SecurityKey(cert), // https://stackoverflow.com/questions/46294373/net-core-issuersigningkey-from-file-for-jwt-bearer-authentication
            ValidAudience = builder.Configuration["IdentityStore:Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = true, // JWTs are required to have "exp" property set
            ValidateLifetime = true, // the "exp" will be validated
            ValidateIssuerSigningKey = true
        };
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

builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IJournalDbService, JournalDbService>();
builder.Services.AddScoped<IResourceAccessHelper, ResourceAccessHelper>();
builder.Services.AddScoped<ISyncDbService, SyncDbService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddDbContext<JournalyDbContext>(); // Do not do this. This API uses concurrency a ton and this will cause race conditions
builder.Services.AddTransient<IDbFactory, DbFactory>(); // Use this instead 

var app = builder.Build();

 
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();