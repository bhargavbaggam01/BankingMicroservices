using Banking.AccountService.Clients;
using Banking.AccountService.Repositories;
using Banking.Common.DTOs;
using Banking.Common.Exceptions;
using Banking.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Banking.AccountService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _repository;
    private readonly ICustomerClient _customerClient;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(
        IAccountRepository repository,
        ICustomerClient customerClient,
        ILogger<AccountsController> logger)
    {
        _repository = repository;
        _customerClient = customerClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAll()
    {
        var accounts = await _repository.GetAllAsync();
        return Ok(accounts);
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<AccountDetailsDto>> GetAccountDetails(int accountId)
    {
        var account = await _repository.GetByIdAsync(accountId);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {accountId} not found");
        }

        // Fetch customer details via Refit
        CustomerDetailsDto? customer = null;
        try
        {
            customer = await _customerClient.GetCustomerById(account.CustomerId);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Customer {CustomerId} not found for account {AccountId}", 
                account.CustomerId, accountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customer {CustomerId} for account {AccountId}", 
                account.CustomerId, accountId);
        }

        var accountDetails = new AccountDetailsDto
        {
            AccountId = account.AccountId,
            CustomerId = account.CustomerId,
            Balance = account.Balance,
            Customer = customer
        };

        return Ok(accountDetails);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Create([FromBody] Account account)
    {
        if (account.CustomerId <= 0)
        {
            throw new ValidationException("Valid CustomerId is required");
        }

        // Verify customer exists
        try
        {
            await _customerClient.GetCustomerById(account.CustomerId);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Customer with ID {account.CustomerId} not found");
        }

        if (account.Balance < 0)
        {
            throw new ValidationException("Initial balance cannot be negative");
        }

        var createdAccount = await _repository.CreateAsync(account);
        return CreatedAtAction(nameof(GetAccountDetails), new { accountId = createdAccount.AccountId }, createdAccount);
    }

    [HttpPost("{accountId}/add-money")]
    public async Task<ActionResult<Account>> AddMoney(int accountId, [FromBody] MoneyTransactionRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ValidationException("Amount must be greater than zero");
        }

        var account = await _repository.GetByIdAsync(accountId);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {accountId} not found");
        }

        // Verify customer exists via Refit
        try
        {
            await _customerClient.GetCustomerById(request.CustomerId);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found");
        }

        // Verify the customer owns this account
        if (account.CustomerId != request.CustomerId)
        {
            throw new ValidationException("Customer does not own this account");
        }

        account.Balance += request.Amount;
        await _repository.UpdateAsync(accountId, account);

        _logger.LogInformation("Added {Amount} to account {AccountId}. New balance: {Balance}", 
            request.Amount, accountId, account.Balance);

        return Ok(account);
    }

    [HttpPost("{accountId}/withdraw-money")]
    public async Task<ActionResult<Account>> WithdrawMoney(int accountId, [FromBody] MoneyTransactionRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ValidationException("Amount must be greater than zero");
        }

        var account = await _repository.GetByIdAsync(accountId);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {accountId} not found");
        }

        // Verify customer exists via Refit
        try
        {
            await _customerClient.GetCustomerById(request.CustomerId);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found");
        }

        // Verify the customer owns this account
        if (account.CustomerId != request.CustomerId)
        {
            throw new ValidationException("Customer does not own this account");
        }

        // Check sufficient funds
        if (account.Balance < request.Amount)
        {
            throw new InsufficientFundsException(
                $"Account {accountId} has insufficient funds. Available: {account.Balance}, Requested: {request.Amount}");
        }

        account.Balance -= request.Amount;
        await _repository.UpdateAsync(accountId, account);

        _logger.LogInformation("Withdrew {Amount} from account {AccountId}. New balance: {Balance}", 
            request.Amount, accountId, account.Balance);

        return Ok(account);
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(int accountId)
    {
        var exists = await _repository.ExistsAsync(accountId);
        if (!exists)
        {
            throw new NotFoundException($"Account with ID {accountId} not found");
        }

        await _repository.DeleteAsync(accountId);
        return NoContent();
    }

    [HttpDelete("customer/{customerId}")]
    public async Task<IActionResult> DeleteAccountsByCustomerId(int customerId)
    {
        var deletedCount = await _repository.DeleteByCustomerIdAsync(customerId);
        
        _logger.LogInformation("Deleted {Count} accounts for customer {CustomerId}", deletedCount, customerId);
        
        return Ok(new { DeletedCount = deletedCount, Message = $"Deleted {deletedCount} accounts for customer {customerId}" });
    }
}

public class MoneyTransactionRequest
{
    public decimal Amount { get; set; }
    public int CustomerId { get; set; }
}
