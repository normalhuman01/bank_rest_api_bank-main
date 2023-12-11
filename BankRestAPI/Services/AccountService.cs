using BankRestAPI.Data;
using BankRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestAPI.Services
{
    public class AccountService : IEntityService<Account>
    {

        private BankDbContext _dbContext;

        public AccountService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // cambiar transfer por account
        public async Task<Account> Create(Account entity)
        {
            await _dbContext.Account.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Guid id)
        {
            var account = await _dbContext.Account.FindAsync(id);
            _dbContext.Account.Remove(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            //return await _dbContext.Account.ToListAsync();

            return await _dbContext.Account
              .Include(a => a.Customer)
              .Include(a => a.Bank)
              .ToListAsync();
        }

        public async Task<Account?> GetById(Guid id)
        {
            return await _dbContext.Account
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.Id == id);

        }

        public async Task<Account> Update(Account entity)
        {
            _dbContext.Account.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Account?> GetByNumber(string number)
        {
            return await _dbContext.Account
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.Number == number);

        }

        public async Task<IEnumerable<Transfer>> GetTransfersByAccountNumber(string number, bool sent, bool received)
        {

            if (sent && received)
            {
                var transfers = await _dbContext.Transfer
                    .Where(t => t.FromAccount.Number == number || t.ToAccount.Number == number)
                    .ToListAsync();
                return transfers;
            }
            else if (sent)
            {
                var transfers = await _dbContext.Transfer
                        .Where(t => t.FromAccount.Number == number)
                        .ToListAsync();
                return transfers;
            }
            else
            {
                var transfers = await _dbContext.Transfer
                    .Where(t => t.ToAccount.Number == number)
                    .ToListAsync();

                return transfers;
            }

        }
    }
}
