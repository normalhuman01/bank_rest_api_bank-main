using BankRestAPI.Data;
using BankRestAPI.DTO;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class AccountController : Controller
    {
        private readonly BankDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly AccountService _accountService;
        private readonly BankService _bankService;
        private readonly CustomerService _customerService;
        

        public AccountController(BankDbContext dbContext, ILogger<AccountController> logger, AccountService accountService, BankService bankService, CustomerService customerService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _accountService = accountService;
            _bankService = bankService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            


            return Ok(await _accountService.GetAll());
        }


        [HttpGet("{number}")]
        public async Task<IActionResult> GetAccount(string number)
        {
            var account = await _accountService.GetByNumber(number);

            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }


        [HttpGet("{number}/transfers")]
        public async Task<IActionResult> GetTransfersByAccountNumber(string number, bool sent, bool received)
        {
            try
            {
                var validationResult = await ValidateAccountTransfers(number, sent, received);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ErrorMessage);
                }

                var transfers = await _accountService.GetTransfersByAccountNumber(number, sent, received);

                return Ok(transfers);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        [HttpPost]
        public async Task<IActionResult> AddAccount(AccountDTO account)
        {
            try
            {

                if (ContainsNullOrEmpty(account) || AccountExists(account) || !ValidateBankAndCustomer(account))
                {
                    return BadRequest();
                }

                if (account.Balance < 0)
                {
                    return BadRequest("El saldo de la cuenta no puede ser negativo");
                }

                return StatusCode(201, await Create(account));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{number}")]
        public async Task<IActionResult> UpdateAccount(string number, decimal balance)
        {
            var account = await _accountService.GetByNumber(number);
            if(account == null)
            {
                return NotFound($"Cuenta Nro. {number} no existe.");
            }
            if(balance <= 0)
            {
                return BadRequest("El saldo no puede ser menor o igual a 0.");
            }

            account.Balance = balance;
            await _accountService.Update(account);
            return Ok(account);
        }
        
        [HttpDelete("{number}")]
        public async Task<IActionResult> DeleteAccount(string number)
        {
            var account = await _accountService.GetByNumber(number);

            if (account == null) { return NotFound($"Account with id {number} not found"); }

            await _accountService.Delete(account.Id);

            return Ok(await _accountService.GetAll());
        }


        private bool ContainsNullOrEmpty(AccountDTO account)
        {
            if (account == null)
            {
                _logger.LogError("Account is null");
                return true;
            }
            if (string.IsNullOrEmpty(account.Currency))
            {
                _logger.LogError("AccountCurrrency is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(account.CustomerDocumentNumber))
            {
                _logger.LogError("AccountCustomerDocumentNumber is null or empty");
                return true;
            }

            return false;
        }

        private bool AccountExists(AccountDTO account)
        {
            if (_dbContext.Account.Any(a => a.Number == account.Number))
            {
                _logger.LogError($"Account with Number {account.Number} already exists");
                return true;
            }
            return false;
        }

        private bool ValidateBankAndCustomer(AccountDTO account)
        {
            var bank = _bankService.GetByCode(account.BankCode);
            var customer = _customerService.GetById(account.CustomerDocumentNumber);
            if (bank.Result == null)
            {
                _logger.LogError($"Bank with code {account.BankCode} does not exist.");
                return false;
            }
            if (customer.Result == null)
            {
                _logger.LogError($"Customer with Document Number {account.CustomerDocumentNumber} does not exist.");
                return false;
            }
            return true;
        }

        private async Task<ValidationResult> ValidateAccountTransfers(string accountNumber, bool sent, bool received)
        {
            var validationResult = new ValidationResult();

            validationResult.ErrorMessage = HasNullOrEmpty(accountNumber, sent, received);

            if (validationResult.ErrorMessage != "OK")
            {
                return validationResult;
            }

            var account = await _accountService.GetByNumber(accountNumber);

            if (account == null)
            {
                validationResult.ErrorMessage = $"La cuenta Nro. {accountNumber} no existe.";
                return validationResult;
            }

            validationResult.Account = account;
            validationResult.IsValid = true;
            return validationResult;
        }

        private string HasNullOrEmpty(string accountNumber, bool sent, bool received)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return "El número de cuenta no puede estar vacío.";
            }
            if (!sent && !received)
            {
                return "Debe seleccionar al menos un tipo de transferencia (enviadas o recibidas).";
            }

            return "OK";
        }


        private async Task<Account> Create(AccountDTO account)
        {
            Account entity = new Account();
            entity.Number = account.Number;
            entity.Balance = account.Balance;
            entity.Currency = account.Currency;
            entity.Customer = await _customerService.GetById(account.CustomerDocumentNumber);
            entity.Bank = await _bankService.GetByCode(account.BankCode);
            entity = await _accountService.Create(entity);
            return entity;
        }



    }


}
