using Banking.Common.Models;
using System.Collections.Concurrent;

namespace Banking.AccountService.Repositories;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int accountId);
    Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId);
    Task<Account> CreateAsync(Account account);
    Task<Account?> UpdateAsync(int accountId, Account account);
    Task<bool> DeleteAsync(int accountId);
    Task<int> DeleteByCustomerIdAsync(int customerId);
    Task<bool> ExistsAsync(int accountId);
}

public class AccountRepository : IAccountRepository
{
    private readonly ConcurrentDictionary<int, Account> _accounts = new();
    private int _nextId = 1;

    public Task<IEnumerable<Account>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Account>>(_accounts.Values.ToList());
    }

    public Task<Account?> GetByIdAsync(int accountId)
    {
        _accounts.TryGetValue(accountId, out var account);
        return Task.FromResult(account);
    }

    public Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId)
    {
        var accounts = _accounts.Values.Where(a => a.CustomerId == customerId).ToList();
        return Task.FromResult<IEnumerable<Account>>(accounts);
    }

    public Task<Account> CreateAsync(Account account)
    {
        account.AccountId = _nextId++;
        _accounts.TryAdd(account.AccountId, account);
        return Task.FromResult(account);
    }

    public Task<Account?> UpdateAsync(int accountId, Account account)
    {
        if (_accounts.TryGetValue(accountId, out var existingAccount))
        {
            account.AccountId = accountId;
            _accounts.TryUpdate(accountId, account, existingAccount);
            return Task.FromResult<Account?>(account);
        }
        return Task.FromResult<Account?>(null);
    }

    public Task<bool> DeleteAsync(int accountId)
    {
        return Task.FromResult(_accounts.TryRemove(accountId, out _));
    }

    public Task<int> DeleteByCustomerIdAsync(int customerId)
    {
        var accountsToDelete = _accounts.Values.Where(a => a.CustomerId == customerId).ToList();
        int deletedCount = 0;

        foreach (var account in accountsToDelete)
        {
            if (_accounts.TryRemove(account.AccountId, out _))
            {
                deletedCount++;
            }
        }

        return Task.FromResult(deletedCount);
    }

    public Task<bool> ExistsAsync(int accountId)
    {
        return Task.FromResult(_accounts.ContainsKey(accountId));
    }
}
