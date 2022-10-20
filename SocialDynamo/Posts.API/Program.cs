using Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.API.Queries;
using Posts.Infrastructure.Persistence;
using Posts.Infrastructure.Repositories;
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

//Posts services
builder.Services.AddTransient<IPostsQueries, PostsQueries>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IFuzzySearch, PostRepository>();

//Infrastructure services
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<PostsDbContext>(options =>
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

//Add Serilog
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
