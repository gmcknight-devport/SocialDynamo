using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Queries;
using Account.API.Services;
using Account.Domain.Repositories;
using Account.Infrastructure.Persistence;
using Common;
using Common.OptionsConfig;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddKeyPerFile("/mnt/secrets-account", optional: true, reloadOnChange: true)
    .AddKeyPerFile("/mnt/secrets-base", optional: true, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(x =>
    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//Connection IOptions
ConnectionOptions connOptions = new() { ServiceBus = configuration["ServiceBus"], AzureAccountDb = configuration["AzureAccountDb"] };
IOptions<ConnectionOptions> connIOptions = Options.Create(connOptions);
builder.Services.AddSingleton(connIOptions);

//Jwt IOptions
JwtOptions jwtOptions = new() { JwtAudience = configuration["JwtAudience"], JwtIssuer = configuration["JwtIssuer"], 
    JwtSecret = configuration["JwtSecret"] };
IOptions<JwtOptions> jwtIOptions = Options.Create(jwtOptions);
builder.Services.AddSingleton(jwtIOptions);

//Account services
builder.Services.AddTransient<IProfileQueries, ProfileQueries>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFollowerRepository, FollowerRepository>();
builder.Services.AddTransient<IFuzzySearch, UserRepository>();

//Auth services
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();

//Infrastructure services
builder.Services.AddDbContext<AccountDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlServer(builder.Configuration["AzureAccountDb"]);
    else
        options.UseSqlServer(configuration["AzureAccountDb"]);
});

//Add Mediator
builder.Services.AddScoped<Mediator>();
builder.Services.AddMediatR(typeof(Program).Assembly);

//Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer", options =>
{    
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

builder.Services.AddEndpointsApiExplorer();

//Add swagger with authorization
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Add serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI().UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();