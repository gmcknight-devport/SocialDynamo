using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Queries;
using Account.API.Services;
using Account.Infrastructure.Persistence;
using Account.Models.Users;
using Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.development.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//Account services
builder.Services.AddTransient<IProfileQueries, ProfileQueries>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFollowerRepository, FollowerRepository>();
builder.Services.AddTransient<IFuzzySearch, UserRepository>();

//Auth services
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();

//Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders();

//Infrastructure services
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AccountDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb"));
    });
}

//Add Mediator
builder.Services.AddSingleton<Mediator>();
builder.Services.AddMediatR(typeof(Program).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
