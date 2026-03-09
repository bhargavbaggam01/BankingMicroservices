using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

var builder = WebApplication.CreateBuilder(args);


// Add Ocelot configuration from ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot with Eureka Provider
builder.Services.AddOcelot(builder.Configuration)
    .AddEureka();

var app = builder.Build();

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
