using Banking.Common.DTOs;
using Refit;

namespace Banking.AccountService.Clients;

public interface ICustomerClient
{
    [Get("/api/customers/{id}")]
    Task<CustomerDetailsDto> GetCustomerById(int id);
}
