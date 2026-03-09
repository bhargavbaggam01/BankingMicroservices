using Banking.Common.Exceptions;
using Banking.Common.Models;
using Banking.CustomerService.Clients;
using Banking.CustomerService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Banking.CustomerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;
    private readonly IAccountClient _accountClient;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerRepository repository,
        IAccountClient accountClient,
        ILogger<CustomersController> logger)
    {
        _repository = repository;
        _accountClient = accountClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await _repository.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {id} not found");
        }
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ValidationException("Customer name is required");
        }

        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            throw new ValidationException("Customer email is required");
        }

        var createdCustomer = await _repository.CreateAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> Update(int id, [FromBody] Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ValidationException("Customer name is required");
        }

        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            throw new ValidationException("Customer email is required");
        }

        var updatedCustomer = await _repository.UpdateAsync(id, customer);
        if (updatedCustomer == null)
        {
            throw new NotFoundException($"Customer with ID {id} not found");
        }

        return Ok(updatedCustomer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException($"Customer with ID {id} not found");
        }

        // Delete associated accounts first using Refit client
        try
        {
            _logger.LogInformation("Attempting to delete accounts for customer {CustomerId}", id);
            var response = await _accountClient.DeleteAccountsByCustomerId(id);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to delete accounts for customer {CustomerId}. Status: {StatusCode}", 
                    id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AccountService to delete accounts for customer {CustomerId}", id);
            // Continue with customer deletion even if account deletion fails
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
