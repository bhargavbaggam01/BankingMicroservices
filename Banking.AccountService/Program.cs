using Banking.AccountService.Clients;
using Banking.AccountService.Repositories;
using Banking.Common.Middleware;
using Refit;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration.ConfigServer;

var builder = WebApplication.CreateBuilder(args);

// Optional: add Steeltoe Config Server only when explicitly enabled
if (Environment.GetEnvironmentVariable("USE_CONFIG_SERVER") == "true")
{
    builder.Configuration.AddConfigServer();
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add Steeltoe Service Discovery
builder.Services.AddServiceDiscovery(o => o.UseEureka());

// Register Repository
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();

builder.Services.AddRefitClient<ICustomerClient>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri("http://CUSTOMER-SERVICE"))
    .AddServiceDiscovery();

// Use environment variable if set (used by Docker), otherwise default to localhost:5002 for local runs
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(urls))
{
    urls = "http://0.0.0.0:5002";
}
builder.WebHost.UseUrls(urls);
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
