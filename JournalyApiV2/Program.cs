using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();