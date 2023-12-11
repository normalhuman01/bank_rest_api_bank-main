using BankRestAPI.Data;
using BankRestAPI.DTO;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class TransferController : Controller
    {
        private readonly ILogger<TransferController> _logger;
        private readonly BankDbContext _bankDbContext;
        private readonly TransferService _transferService;
        private readonly BankService _bankService;
        private readonly AccountService _accountService;
        private readonly CustomerService _customerService;

        public TransferController(ILogger<TransferController> logger, BankDbContext bankDbContext, TransferService transferService, BankService bankService, CustomerService customerService, AccountService accountService)
        {
            _logger = logger;
            _bankDbContext = bankDbContext;
            _transferService = transferService;
            _bankService = bankService;
            _accountService = accountService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransfers()
        {
            return Ok(await _transferService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransfer(Guid id)
        {
            var transfer = await _transferService.GetById(id);

            if (transfer == null)
            {
                return NotFound($"Transfer with id {id} not found");
            }

            return Ok(transfer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransfer(TransferDTO transfer)
        {

            // validation methods
            var validationResult = await ValidateTransferAsync(transfer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ErrorMessage);
            }

            await UpdateBalance(transfer, validationResult.FromAccount, validationResult.ToAccount);

            return StatusCode(201, await Create(transfer));
        }


        [HttpPut("TransactionState/{id:guid}")]
        public async Task<IActionResult> UpdateTransactionState(Guid id, string transactionState)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return NotFound("");
            }
            transfer.TransactionState = transactionState;
            await _transferService.Update(transfer);
            return Ok(transfer);
        }


        [HttpGet("TransactionState/{id:guid}")]
        public async Task<IActionResult> GetTransactionState(Guid id)
        {
            var transfer = await _transferService.GetById(id);
            if (transfer == null)
            {
                return NotFound();
            }
            return Ok(transfer.TransactionState);
        }


        private async Task<ValidationResult> ValidateTransferAsync(TransferDTO transfer)
        {

            var validationResult = new ValidationResult();

            validationResult.ErrorMessage = ContainsNullOrEmpty(transfer);
            if (validationResult.ErrorMessage != "OK")
            {
                return validationResult;
            }

            var fromBank = await _bankService.GetByCode(transfer.FromBankCode);
            var toBank = await _bankService.GetByCode(transfer.ToBankCode);
            var fromAccount = await _accountService.GetByNumber(transfer.FromAccountNumber);
            var toAccount = await _accountService.GetByNumber(transfer.ToAccountNumber);
            var fromCustomer = await _customerService.GetById(transfer.FromCustomerDocNumber);
            var toCustomer = await _customerService.GetById(transfer.ToCustomerDocNumber);

            validationResult.ErrorMessage = validateBanks(transfer, fromBank, toBank);
            if (validationResult.ErrorMessage != "OK")
            {
                return validationResult;
            }

            if (fromAccount == null || toAccount == null)
            {
                validationResult.ErrorMessage = "Al menos una de las cuentas no existe.";
                return validationResult;
            }

            if (!validateTransferAmount(transfer, fromAccount))
            {
                throw new Exception($"Insuficiencia de fondos: {fromAccount.Balance}.");
            }

            if (!validateCustomers(fromCustomer, toCustomer))
            {
                validationResult.ErrorMessage = "Al menos uno de los clientes no existe.";
                return validationResult;
            }

            // Que el account sea del banco
            if (fromAccount.Bank.Code != fromBank.Code)
            {
                validationResult.ErrorMessage = $"La cuenta Nro. {fromAccount.Number} no se encuentra registrada en el ban nro. {fromBank.Name}.";
                return validationResult;
            }
            if (toAccount.Bank.Code != toBank.Code)
            {
                validationResult.ErrorMessage = $"La cuenta de origen Nro. {toAccount.Number} no se encuentra registrada en el banco de origen {toBank.Name}.";
                return validationResult;
            }

            // Que el account pertenezca al customer 
            if (fromAccount.Customer.DocumentNumber != fromCustomer.DocumentNumber)
            {
                validationResult.ErrorMessage = $"La cuenta Nro. {fromAccount.Number} no corresponde al cliente con Document {fromCustomer.DocumentNumber}.";
                return validationResult;
            }
            if (toAccount.Customer.DocumentNumber != toCustomer.DocumentNumber)
            {
                validationResult.ErrorMessage = $"La cuenta Nro. {toAccount.Number} no corresponde al cliente con Document {toCustomer.DocumentNumber}.";
                return validationResult;
            }

            if(transfer.Currency != fromAccount.Currency)
            {
                validationResult.ErrorMessage = $"La moneda de transferencia debe ser la misma que en la cuenta: {transfer.Currency} -> {fromAccount.Currency}.";
                return validationResult;

            }

            if (transfer.Currency != toAccount.Currency)
            {
                validationResult.ErrorMessage = $"La moneda de transferencia debe ser la misma que en la cuenta: {transfer.Currency} -> {toAccount.Currency}.";
                return validationResult;

            }

            validationResult.FromAccount = fromAccount;
            validationResult.ToAccount = toAccount;
            validationResult.IsValid = true;
            return validationResult;
        }

        private string validateBanks(TransferDTO transfer, Bank? fromBank, Bank? toBank)
        {
            if (fromBank == null || toBank == null)
            {
                return "Al menos uno de los bancos no existe";
            }

            if (fromBank == toBank)
            {
                throw new Exception("Bancos NO deben ser Iguales");
            }

            if (transfer.FromBankName.ToUpper() != fromBank.Name.ToUpper())
            {
                return $"Nombre de Banco {transfer.FromBankName} no corresponde al código {fromBank.Code}";
            }
            if (transfer.ToBankName.ToUpper() != toBank.Name.ToUpper())
            {
                return $"Nombre de Banco {transfer.ToBankName} no corresponde al código {toBank.Code}";
            }


            return "OK";
        }

        private async Task UpdateBalance(TransferDTO transfer, Account fromAccount, Account toAccount)
        {
            fromAccount.Balance = fromAccount.Balance - transfer.Amount;
            toAccount.Balance = toAccount.Balance + transfer.Amount;

            await _accountService.Update(fromAccount);
            await _accountService.Update(toAccount);
        }

        private string ContainsNullOrEmpty(TransferDTO transfer)
        {
            string msg;

            if (transfer == null)
            {
                msg = "Transfer object is null";
                _logger.LogError(msg);
                return msg;
            }

            if (string.IsNullOrEmpty(transfer.Currency))
            {
                msg = "Currency es null o empty";
                _logger.LogError(msg);
                return msg;
            }


            // origin data 
            if (string.IsNullOrEmpty(transfer.FromBankName))
            {
                msg = "Origin Bank Name es null o empty";
                _logger.LogError(msg);
                return msg;
            }

            if (string.IsNullOrEmpty(transfer.FromBankCode))
            {
                msg = "Origin Bank Code es null o empty";
                _logger.LogError(msg);
                return msg;
            }
            if (string.IsNullOrEmpty(transfer.FromCustomerDocNumber))
            {
                msg = "Origin Document Number es null o empty";
                _logger.LogError(msg);
                return msg;
            }

            if (string.IsNullOrEmpty(transfer.FromAccountNumber))
            {
                msg = "Origin Account Number es null o empty";
                _logger.LogError(msg);
                return msg;
            }


            // receiver data
            if (string.IsNullOrEmpty(transfer.ToBankName))
            {
                msg = "Receiver Bank Name es null or empty";
                _logger.LogError(msg);
                return msg;
            }

            if (string.IsNullOrEmpty(transfer.ToBankCode))
            {
                msg = "Receiver Bank Code es null or empty";
                _logger.LogError(msg);
                return msg;
            }
            if (string.IsNullOrEmpty(transfer.ToCustomerDocNumber))
            {
                msg = "Receiver Document Number es null or empty";
                _logger.LogError(msg);
                return msg;
            }

            if (string.IsNullOrEmpty(transfer.ToAccountNumber))
            {
                msg = "Receiver Account Number es null or empty";
                _logger.LogError(msg);
                return msg;
            }
            return "OK";
        }

        private bool validateCustomers(Customer? fromCustomer, Customer? toCustomer)
        {
            if (fromCustomer == null || toCustomer == null)
            {
                _logger.LogError("Al menos uno de los clientes no existe");
                return false;
            }
            return true;
        }

        private bool validateTransferAmount(TransferDTO transfer, Account fromAccount)
        {
            var amount = transfer.Amount;

            if (amount <= 0)
            {
                _logger.LogError($"Monto de transferencia menor o igual a 0: ${amount}");
                throw new Exception($"Monto inválido: {amount}");
            }
            if (amount > fromAccount.Balance)
            {
                _logger.LogError($"Monto de transferencia es mayor al balance de cuenta: {transfer.Currency} {amount}");
                return false;
            }

            return true;
        }

        private async Task<Transfer> Create(TransferDTO transfer)
        {
            Transfer entity = new Transfer();
            entity.FromBankName = transfer.FromBankName;
            entity.ToBankName = transfer.ToBankName;
            entity.FromBank = await _bankService.GetByCode(transfer.FromBankCode);
            entity.ToBank = await _bankService.GetByCode(transfer.ToBankCode);
            entity.FromAccount = await _accountService.GetByNumber(transfer.FromAccountNumber);
            entity.ToAccount = await _accountService.GetByNumber(transfer.ToAccountNumber);
            entity.FromCustomer = await _customerService.GetById(transfer.FromCustomerDocNumber);
            entity.ToCustomer = await _customerService.GetById(transfer.ToCustomerDocNumber); ;
            entity.OperationDate = DateTime.Now.ToUniversalTime();
            entity.Currency = transfer.Currency;
            entity.TransactionState = "FINALIZADO";
            entity.Amount = transfer.Amount;

            await _transferService.Create(entity);
            return entity;
        }
    }


    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public Account? FromAccount { get; set; }
        public Account? ToAccount { get; set; }
        public Account? Account { get; set; }
        public Customer? Customer { get; set; }
    }
}
