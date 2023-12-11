namespace BankRestAPI.DTO
{
    public class AccountDTO
    {
        public string Number { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; } = default!;
        public string CustomerDocumentNumber { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
    }
}
