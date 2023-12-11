namespace BankRestAPI.DTO
{
    public class TransferDTO
    {

        //datos de origen
        public string FromBankName { get; set; } = default!;
        public string FromBankCode { get; set; } = default!;
        public string FromAccountNumber { get; set; } = default!;
        public string FromCustomerDocNumber { get; set; } = default!;

        // Datos de destino
        public string ToBankName { get; set; } = default!;
        public string ToBankCode { get; set; } = default!;
        public string ToAccountNumber { get; set; } = default!;
        public string ToCustomerDocNumber { get; set; } = default!;

        public string Currency { get; set; } = default!;
        public decimal Amount { get; set; } = default!;
    }
}
