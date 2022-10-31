using Media.API.IntegrationEvents;
using Media.API.Queries;
using MediatR;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddTransient<IMediaQueries, MediaQueries>();
builder.Services.AddSingleton<Mediator>();
builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddHostedService<DeleteUserIntegrationEventHandler>();
builder.Services.AddHostedService<NewUserIntegrationEventHandler>();
builder.Services.AddHostedService<PostDeletedIntegrationEventHandler>();

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
