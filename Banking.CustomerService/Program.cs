using Banking.Common.Middleware;
using Banking.CustomerService.Clients;
using Banking.CustomerService.Repositories;
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
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();

// Registered Refit Client for AccountService
builder.Services.AddRefitClient<IAccountClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://account-service:80"));


builder.WebHost.UseUrls("http://0.0.0.0:80");
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
