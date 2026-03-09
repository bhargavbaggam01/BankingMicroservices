namespace Banking.Common.DTOs;

public class AccountDetailsDto
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public CustomerDetailsDto? Customer { get; set; }
}
