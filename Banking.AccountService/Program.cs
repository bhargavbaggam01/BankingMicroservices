using Banking.AccountService.Clients;
using Banking.AccountService.Repositories;
using Banking.Common.Middleware;
using Refit;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Extensions.Configuration.ConfigServer;

var builder = WebApplication.CreateBuilder(args);

// Add Steeltoe Config Server
builder.Configuration.AddConfigServer();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add Steeltoe Service Discovery
builder.Services.AddServiceDiscovery(o => o.UseEureka());

// Register Repository
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();

// Register Refit Client for CustomerService
// Use explicit port for now - Eureka discovery will be handled internally
builder.Services.AddRefitClient<ICustomerClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://customer-service:80"));

builder.WebHost.UseUrls("http://0.0.0.0:80");
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
