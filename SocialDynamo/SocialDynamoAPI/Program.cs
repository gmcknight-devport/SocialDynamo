using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Add ocelot service
builder.Services.AddOcelot();

var app = builder.Build();

app.UseOcelot();

app.Run();
