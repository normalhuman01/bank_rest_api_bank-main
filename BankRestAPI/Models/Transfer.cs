namespace BankRestAPI.Models
{
    public class Transfer
    {
        public Guid Id { get; set; }

        //datos de origen
        public string FromBankName { get; set; } = default!;
        public Bank FromBank { get; set; } = default!;
        public Account FromAccount { get; set; } = default!;
        public Customer FromCustomer { get; set; } = default!;

        // Datos de destino
        public string ToBankName { get; set; } = default!;
        public Bank ToBank { get; set; } = default!;
        public Account ToAccount { get; set; } = default!;
        public Customer ToCustomer { get; set; } = default!;

        public DateTime OperationDate { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal Amount { get; set; } = default!;
        public string? TransactionState { get; set; } = default!;

    }
}
