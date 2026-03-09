using Banking.Common.Middleware;
using Banking.CustomerService.Clients;
using Banking.CustomerService.Repositories;
using Refit;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Steeltoe Config Server
builder.Configuration.AddConfigServer();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Enable Eureka discovery
builder.Services.AddServiceDiscovery(o => o.UseEureka());

// Register repository
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();

// Refit client using Eureka discovery
builder.Services.AddRefitClient<IAccountClient>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri("http://ACCOUNT-SERVICE"))
    .AddServiceDiscovery();

// Configure URLs (Docker or local)
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(urls))
{
    urls = "http://0.0.0.0:5001";
}

builder.WebHost.UseUrls(urls);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global exception middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();