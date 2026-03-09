using Refit;

namespace Banking.CustomerService.Clients;

public interface IAccountClient
{
    [Delete("/api/accounts/customer/{customerId}")]
    Task<IApiResponse> DeleteAccountsByCustomerId(int customerId);
}
