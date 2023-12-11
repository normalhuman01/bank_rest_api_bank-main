using BankRestAPI.Data;
using BankRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestAPI.Services
{
    public class CustomerService
    {
        private readonly BankDbContext _dbContext;

        public CustomerService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> Create(Customer entity)
        {
            await _dbContext.Customer.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(string DocumentNumber)
        {
            Customer customer = await _dbContext.Customer.FindAsync(DocumentNumber);
            _dbContext.Customer.Remove(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _dbContext.Customer.ToListAsync();
        }

        public async Task<Customer?> GetById(string DocumentNumber)
        {
            return await _dbContext.Customer.FindAsync(DocumentNumber);
        }


        public async Task<Customer> Update(Customer entity)
        {
            _dbContext.Customer.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Transfer>> GetTransfersByDocumentNumber(string documentNumber, bool sent, bool received)
        {

            if (sent && received)
            {
                var transfers = await _dbContext.Transfer
                    .Where(t => t.FromCustomer.DocumentNumber == documentNumber || t.ToCustomer.DocumentNumber == documentNumber)
                    .ToListAsync();
                return transfers;
            }

            else if (sent)
            {
                var transfers = await _dbContext.Transfer
                        .Where(t => t.FromCustomer.DocumentNumber == documentNumber)
                        .ToListAsync();
                return transfers;
            }
            else
            {
                var transfers = await _dbContext.Transfer
                    .Where(t => t.ToCustomer.DocumentNumber == documentNumber)
                    .ToListAsync();

                return transfers;
            }

        }
    }
}
