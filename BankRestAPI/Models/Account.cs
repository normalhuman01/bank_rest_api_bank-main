namespace BankRestAPI.Models
{
    public class Account
    {
        public Guid Id { get; set; } = default!;
        public string Number { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; } = default!;

        //Navigation property to Customer
        public Customer? Customer { get; set; } = default!;

        // Navigation properties to Bank
        public Bank? Bank { get; set; } = default!;

    }
}
