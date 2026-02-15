using Microsoft.EntityFrameworkCore;
using Task_1.DbCon;
using Task_1.Dto;
using Task_1.Entities;

namespace Task_1.Services
{
    public class CustomerService
    {
        private readonly IDbContextFactory<CustomerDbContext> _dbFactory;

        public CustomerService(IDbContextFactory<CustomerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Customer>> GetCustomerEditableAsync(CancellationToken ct = default)
        {
            using var context = await _dbFactory.CreateDbContextAsync(ct);
            var result = context.Customer.Select(c => new Customer
            {
                Cust_Id = c.Cust_Id,
                Cust_Code = c.Cust_Code,
                Cust_Name = c.Cust_Name,
                Cust_ContactNo = c.Cust_ContactNo,
                Cust_Email = c.Cust_Email
            }).ToListAsync(ct);

            return await result;
        }

        public async Task InsertCustomerAsync(Customer newDataItem)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var entity = new Customer
            {
                Cust_Code = newDataItem.Cust_Code,
                Cust_Name = newDataItem.Cust_Name,
                Cust_ContactNo = newDataItem.Cust_ContactNo,
                Cust_Email = newDataItem.Cust_Email,
                Created_on = DateTime.Now,
                Created_DateTime = DateTime.Now,
                Updated_on = DateTime.Now,
                Updated_DateTime = DateTime.Now
            };
            await context.Customer.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveCustomerAsync(int dataItem)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var find = await context.Customer.FindAsync(dataItem);
            if (find != null)
            {
                context.Customer.Remove(find);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateCustomerAsync(Customer dto)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var entity = await context.Customer
                .FirstOrDefaultAsync(c => c.Cust_Id == dto.Cust_Id);

            if (entity == null)
                throw new Exception("Customer not found");

            entity.Cust_Name = dto.Cust_Name;
            entity.Cust_ContactNo = dto.Cust_ContactNo;
            entity.Cust_Email = dto.Cust_Email;
            entity.Updated_on = DateTime.Now;
            entity.Updated_DateTime = DateTime.Now;

            await context.SaveChangesAsync();
        }


    }
}

