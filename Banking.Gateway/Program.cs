using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration from ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Steeltoe service discovery (Eureka)
builder.Services.AddServiceDiscovery();

// Add Ocelot with Eureka Provider
builder.Services.AddOcelot(builder.Configuration)
    .AddEureka();

// Use environment variable if set (used by Docker), otherwise default to localhost:5000 for local runs
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(urls))
{
    urls = "http://0.0.0.0:5000";
}
builder.WebHost.UseUrls(urls);

var app = builder.Build();

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
