using Common.API.Infrastructure.Repositories;
using Common.API.Profile.Queries;
using Common.API.Services;
using Common.Domain.Repositories;
using Common.Infrastructure.Persistence;
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
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

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

builder.Services.AddHttpContextAccessor();

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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<Mediator>();

//Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(x =>
    {
        x.Cookie.Name = "token";
        x.Cookie.IsEssential = true;
    })
    .AddJwtBearer("Bearer", options =>
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };

    options.SaveToken = true;
});

builder.Services.AddEndpointsApiExplorer();

//Add swagger with authorization
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();