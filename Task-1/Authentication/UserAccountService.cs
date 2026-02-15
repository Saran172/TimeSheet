using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task_1.DbCon;
using Task_1.Entities;

namespace Task_1.Authentication
{
    public class UserAccountService
    {
        private readonly IDbContextFactory<CustomerDbContext> _dbFactory;
        private readonly PasswordHasher<string> _passwordHasher = new();

        public UserAccountService(IDbContextFactory<CustomerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task<Employee?> GetByUserNameAsync(string userName)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Employee.FirstOrDefaultAsync(x => x.EMP_NAME == userName);
        }

        public bool VerifyPassword(Employee employee, string providedPassword)
        {
            // Do NOT pass a Task here. Pass the actual Employee object.
            var result = _passwordHasher.VerifyHashedPassword(null!, employee.EMP_PASSWORD, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
        //public async Task<Employee?> GetByUserName(string userName)
        //{
        //    using var context = await _dbFactory.CreateDbContextAsync();
        //    // Using FirstOrDefaultAsync to ensure non-blocking behavior
        //    return await context.Employee.FirstOrDefaultAsync(x => x.EMP_NAME == userName);
        //}

        //public bool VerifyPassword(Task<Employee?> employee, string providedPassword)
        //{
        //    // Note: This method doesn't need a DbContext as it only hashes the string
        //    return _passwordHasher.VerifyHashedPassword(null!, employee.Result.EMP_PASSWORD, providedPassword)
        //           == PasswordVerificationResult.Success;
        //}

        public async Task RegisterUserAsync(string userName, string plainPassword, string role)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var hashedPassword = _passwordHasher.HashPassword(null!, plainPassword);
            var newUser = new Employee
            {
                EMP_NAME = userName,
                EMP_PASSWORD = hashedPassword,
                Role = role
            };

            await context.Employee.AddAsync(newUser);
            await context.SaveChangesAsync();
        }

        //private CustomerDbContext _customerDbContext;
        //private readonly PasswordHasher<string> _passwordHasher = new();

        //public UserAccountService(CustomerDbContext customerDbContext)
        //{
        //    _customerDbContext = customerDbContext;
        //}

        //private List<UserAccount> _users;
        //public Employee? GetByUserName(string userName)
        //{
        //    return _customerDbContext.Employee.FirstOrDefault(x => x.EMP_NAME == userName); 
        //}
        //public bool VerifyPassword(Employee employee, string providedPassword)
        //{
        //    return _passwordHasher.VerifyHashedPassword(null, employee.EMP_PASSWORD, providedPassword)
        //           == PasswordVerificationResult.Success;
        //}
        //public void RegisterUser(string userName, string plainPassword, string role)
        //{
        //    var hashedPassword = _passwordHasher.HashPassword(null, plainPassword);
        //    var newUser = new Employee
        //    {
        //        EMP_NAME = userName,
        //        EMP_PASSWORD = hashedPassword,
        //        Role = role
        //    };

        //    _customerDbContext.Employee.Add(newUser);
        //    _customerDbContext.SaveChanges();
        //}

    }
}
