using BankRestAPI.Data;
using BankRestAPI.DTO;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankRestAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]s")]
    public class BankController : Controller
    {
        private readonly ILogger<BankController> _logger;
        private readonly BankService _bankService;
        private readonly BankDbContext _dbContext;

        public BankController(ILogger<BankController> logger,
            BankService bankService, BankDbContext dbContext)
        {
            _logger = logger;
            _bankService = bankService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await _bankService.GetAll());
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetBank(string code)
        {
            var bank = await _bankService.GetByCode(code);

            if (bank == null)
            {
                return NotFound();
            }

            return Ok(bank);
        }


        [HttpPost]
        public async Task<IActionResult> AddBank(BankDTO bankDTO)
        {
            try
            {
                if (ContainsNullOrEmpty(bankDTO) || await BankExists(bankDTO))
                {
                    return BadRequest();
                }

                return StatusCode(201, await Create(bankDTO));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Route("{code}")]
        public async Task<IActionResult> UpdateBank(string code, BankDTO bank)
        {

            var entity = await _bankService.GetByCode(code);

            if (entity == null)
            {
                _logger.LogError($"Bank {entity} is null");
                return BadRequest();
            }

            if (await BankExists(bank))
            {
                return BadRequest();
            }

            await Update(entity, bank);

            return Ok(entity);
        }


        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteBank(string code)
        {
            var bank = await _bankService.GetByCode(code);

            if (bank == null) { return NotFound($"Bank with code {code} not found"); }

            await _bankService.Delete(bank.Id);

            return Ok(await _bankService.GetAll());
        }



        private async Task Update(Bank entity, BankDTO bank)
        {
            if (!string.IsNullOrEmpty(bank.Code))
            {
                entity.Code = bank.Code;
            }
            if (!string.IsNullOrEmpty(bank.Name))
            {
                entity.Name = bank.Name;
            }
            if (!string.IsNullOrEmpty(bank.Address))
            {
                entity.Address = bank.Address;
            }
            await _bankService.Update(entity);
        }

        private bool ContainsNullOrEmpty(BankDTO bank)
        {
            if (bank == null)
            {
                _logger.LogError("Bank object is null");
                return true;
            }
            if (string.IsNullOrEmpty(bank.Code))
            {
                _logger.LogError("BankCode is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(bank.Name))
            {
                _logger.LogError("BankName is null or empty");
                return true;
            }
            if (string.IsNullOrEmpty(bank.Address))
            {
                _logger.LogError("BankAddress is null or empty");
                return true;
            }

            return false;
        }

        private async Task<bool> BankExists(BankDTO bank)
        {
            var entityByCode = await _bankService.GetByCode(bank.Code);
            var entityByName = await _bankService.GetByName(bank.Name);
            var entityByAddress = await _bankService.GetByAddress(bank.Address);

            if (entityByCode != null)
            {
                _logger.LogError($"Code {bank.Code} corresponds to another Bank");
                return true;
            }

            if (entityByName != null)
            {
                _logger.LogError($"Bank {bank.Name} already exists");
                return true;
            }
            if (entityByAddress != null)
            {
                _logger.LogError($"Address {bank.Address} corresponds to another Bank");
                return true;
            }
            return false;
        }

        private async Task<Bank> Create(BankDTO bankDTO)
        {
            Bank bank = new Bank();
            bank.Code = bankDTO.Code;
            bank.Name = bankDTO.Name;
            bank.Address = bankDTO.Address;
            await _bankService.Create(bank);
            return bank;
        }


    }
}
