using Common;
using Common.OptionsConfig;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Posts.API.Queries;
using Posts.Domain.Repositories;
using Posts.Infrastructure.Persistence;
using Posts.Infrastructure.Repositories;
using Serilog;
using System.Text;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddKeyPerFile("/mnt/secrets-posts", optional: true, reloadOnChange: true)
    .AddKeyPerFile("/mnt/secrets-base", optional: true, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(x => 
    x.SerializerSettings.ReferenceLoopHandling =Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//Connection IOptions
ConnectionOptions connOptions = new() { ServiceBus = configuration["ServiceBus"], AzureAccountDb = configuration["AzureAccountDb"] };
IOptions<ConnectionOptions> connIOptions = Options.Create(connOptions);
builder.Services.AddSingleton(connIOptions);

//Posts services
builder.Services.AddTransient<IPostsQueries, PostsQueries>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IFuzzySearch, PostRepository>();

//Infrastructure services
builder.Services.AddDbContext<PostsDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlServer(builder.Configuration["AzurePostsDb"]);
    else
        options.UseSqlServer(configuration["AzurePostsDb"]);
});


//Add Mediator
builder.Services.AddScoped<Mediator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Serilog
builder.Host.UseSerilog();

//Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(x => 
    { 
        x.Cookie.Name = "token"; 
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
});

builder.Services.AddEndpointsApiExplorer();

//Add swagger with authorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
});

builder.Services.AddEndpointsApiExplorer();

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
