using Common.OptionsConfig;
using Media.API.IntegrationEvents;
using Media.API.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var config = new ConfigurationBuilder()
    .AddKeyPerFile("/mnt/secrets-media", optional: true, reloadOnChange: true)
    .AddKeyPerFile("/mnt/secrets-base", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.json");

//if (Directory.Exists("/mnt/secrets-base") && Directory.Exists("/mnt/secrets-media"))
//    config.AddKeyPerFile("/mnt/secrets-base")
//                 .AddKeyPerFile("/mnt/secrets-media");

config.Build();

var configuration = config.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(x =>
    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;

builder.Services.AddTransient<IMediaQueries, MediaQueries>();

builder.Services.AddScoped<Mediator>();
builder.Services.AddMediatR(typeof(Program).Assembly);

//Add IOption service for background services to receive secret values.
ConnectionOptions options = new() { ServiceBus = configuration["ServiceBus"], AzureStorage = configuration["AzureStorage"] };
IOptions<ConnectionOptions> iOptions = Options.Create(options);
builder.Services.AddSingleton(iOptions);

//Background services
builder.Services.AddHostedService<DeleteUserIntegrationEventHandler>();
builder.Services.AddHostedService<NewUserIntegrationEventHandler>();
builder.Services.AddHostedService<PostDeletedIntegrationEventHandler>();

//Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

//Add swagger with authoirzation
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