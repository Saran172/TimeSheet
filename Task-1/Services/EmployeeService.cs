using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Task_1.DbCon;
using Task_1.Entities;
using Task_1.Model;

namespace Task_1.Services
{
    public class EmployeeService
    {
        private readonly IDbContextFactory<CustomerDbContext> _dbFactory;

        public EmployeeService(IDbContextFactory<CustomerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Employee>> GetEmployeeEditableAsync(CancellationToken ct = default)
        {
            using var context = await _dbFactory.CreateDbContextAsync(ct);
            return await context.Employee.ToListAsync(ct);
        }

        public async Task<List<Employee>> GetEmployeeByDesignation()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Employee
                .Where(e => e.EMP_DESIGNATION == "Trainee" ||
                            e.EMP_DESIGNATION == "Dot Net Developer" ||
                            e.EMP_DESIGNATION == "Flutter Developer")
                .ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeeDesignation()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Employee
                .Where(e => e.EMP_DESIGNATION == "Social Media Marketing Executive" ||
                            e.EMP_DESIGNATION == "Testing & Support" ||
                            e.EMP_DESIGNATION == "SAP")
                .ToListAsync();
        }

        public async Task InsertEmployeeAsync(Employee employee)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            try
            {
                var existingEmployee = await context.Employee.FindAsync(employee.EMP_ID);

                if (existingEmployee != null)
                {
                    context.Entry(existingEmployee).CurrentValues.SetValues(employee);
                }
                else
                {
                    context.Employee.Add(employee);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Consider logging ex.Message
            }
        }

        public async Task RemoveEmployeeAsync(int dataItem)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var find = await context.Employee.FindAsync(dataItem);
            if (find != null)
            {
                context.Employee.Remove(find);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeAsync(Employee dataItem, IDictionary<string, object> newValues)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            // Attach the existing item to the new context
            context.Employee.Attach(dataItem);

            foreach (var empl in newValues)
            {
                var propertyInfo = dataItem.GetType().GetProperty(empl.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(dataItem, Convert.ChangeType(empl.Value, propertyInfo.PropertyType));
                }
            }

            context.Employee.Update(dataItem);
            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.Employee.Update(employee);
            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsyncc(Employee employee)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            try
            {
                var existingEmployee = await context.Employee.FindAsync(employee.EMP_ID);
                if (existingEmployee != null)
                {
                    context.Entry(existingEmployee).CurrentValues.SetValues(employee);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
            }
        }

        public async Task<List<Employee>> GetByUserName(string userName)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var emp = await context.Employee.FirstOrDefaultAsync(x => x.EMP_NAME == userName);
            return emp != null ? new List<Employee> { emp } : new List<Employee>();
        }
        //private readonly CustomerDbContext _context;
        //public EmployeeService(CustomerDbContext context)
        //{
        //    _context = context;
        //}
        //public async Task<List<Employee>> GetEmployeeEditableAsync(CancellationToken ct = default)
        //{   
        //    var employee = await _context.Employee.ToListAsync(ct);
        //    return employee;
        //}

        //public async Task<List<Employee>> GetEmployeeByDesignation()
        //{
        //    var employee = await _context.Employee.Where(e => e.EMP_DESIGNATION == "Trainee" || e.EMP_DESIGNATION == "Dot Net Developer" || e.EMP_DESIGNATION == "Flutter Developer").ToListAsync();
        //    return employee;
        //}
        //public async Task<List<Employee>> GetEmployeeDesignation()
        //{
        //    var employee = await _context.Employee.Where(e => e.EMP_DESIGNATION == "Social Media Marketing Executive" || e.EMP_DESIGNATION == "Testing & Support" || e.EMP_DESIGNATION == "SAP").ToListAsync();
        //    return employee;
        //}
        //public async Task InsertEmployeeAsync(Employee employee)
        //{
        //    try
        //    {
        //        var existingEmployee = await _context.Employee
        //                .FindAsync(employee.EMP_ID);

        //        if (existingEmployee != null)
        //        {
        //            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
        //        }
        //        else
        //        {
        //            // Add new employee
        //            _context.Employee.Add(employee);
        //            await _context.SaveChangesAsync();
        //        }


        //    }
        //    catch (Exception ex)
        //    { }

        //}
        //public async Task RemoveEmployeeAsync(int dataItem)
        //{   
        //    var find = await _context.Employee.FindAsync(dataItem);
        //    _context.Employee.Remove(find);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task UpdateEmployeeAsync(Employee dataItem, IDictionary<string, object> newValues)
        //{
        //    foreach (var empl in newValues)
        //    {
        //        var propertyInfo = dataItem.GetType().GetProperty(empl.Key);
        //        if (propertyInfo != null)
        //        {
        //            propertyInfo.SetValue(dataItem, Convert.ChangeType(empl.Value, propertyInfo.PropertyType));
        //        }
        //    }

        //    _context.Employee.Update(dataItem);
        //    await _context.SaveChangesAsync();

        //}
        //public async Task UpdateEmployeeAsync(Employee employee)
        //{
        //    _context.Employee.Update(employee);
        //    await _context.SaveChangesAsync();

        //}
        //public async Task UpdateEmployeeAsyncc(Employee employee)
        //{
        //    try
        //    {
        //        var existingEmployee = await _context.Employee.FindAsync(employee.EMP_ID);
        //        if (existingEmployee != null)
        //        {
        //            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating employee: {ex.Message}");
        //    }
        //}

        //public async Task<List<Employee>> GetByUserName(string userName)
        //{
        //    var emp=  _context.Employee.FirstOrDefault(x => x.EMP_NAME == userName);
        //    return emp != null ? new List<Employee> { emp } : new List<Employee>();
        //}
    }
}

