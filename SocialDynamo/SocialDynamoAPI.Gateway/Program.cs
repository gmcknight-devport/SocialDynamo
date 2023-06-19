using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .AddJsonFile("appsettings.json")
    .AddKeyPerFile("/mnt/secrets-base", optional: true, reloadOnChange: true)
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

//Add authentication
builder.Services.AddAuthentication().AddJwtBearer("Bearer", options =>
{    
    options.SaveToken = true;
    if (builder.Environment.IsDevelopment())
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSecret"]))
        };
    }
    else
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtIssuer"],
            ValidAudience = configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSecret"]))
        };
    }
});

builder.Services.AddOcelot(configuration).AddKubernetes();

var app = builder.Build();

app.UseOcelot().Wait();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.Run();
