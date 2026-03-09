using Banking.Common.Models;
using System.Collections.Concurrent;

namespace Banking.CustomerService.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer?> UpdateAsync(int id, Customer customer);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<int, Customer> _customers = new();
    private int _nextId = 1;

    public Task<IEnumerable<Customer>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Customer>>(_customers.Values.ToList());
    }

    public Task<Customer?> GetByIdAsync(int id)
    {
        _customers.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    public Task<Customer> CreateAsync(Customer customer)
    {
        customer.Id = _nextId++;
        _customers.TryAdd(customer.Id, customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        if (_customers.TryGetValue(id, out var existingCustomer))
        {
            customer.Id = id;
            _customers.TryUpdate(id, customer, existingCustomer);
            return Task.FromResult<Customer?>(customer);
        }
        return Task.FromResult<Customer?>(null);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_customers.TryRemove(id, out _));
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_customers.ContainsKey(id));
    }
}
