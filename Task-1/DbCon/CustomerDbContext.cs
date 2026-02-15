using Microsoft.EntityFrameworkCore;
using Task_1.Entities;

namespace Task_1.DbCon
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Customer> Customer { get; set; } = null!;
        public virtual DbSet<Project> Project { get; set; } = null!;
        public virtual DbSet<Work> Work { get; set; } = null!;
        public virtual DbSet<Employee> Employee { get; set; } = null!;
        public virtual DbSet<WorkApproval> WorkApproval { get; set; } = null!;


    }
}
