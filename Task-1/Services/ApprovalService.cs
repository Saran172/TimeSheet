using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Task_1.DbCon;
using Task_1.Entities;


namespace Task_1.Services
{
    public class ApprovalService
    {
        private readonly IDbContextFactory<CustomerDbContext> _dbFactory;

        public ApprovalService(IDbContextFactory<CustomerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<WorkApproval>> GetWorkApproval()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var workApproval = from workApprovals in context.WorkApproval
                               join emp in context.Employee on workApprovals.Emp_Id equals emp.EMP_ID
                               where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer") && workApprovals.Approval_Status == "Hold"
                               select new WorkApproval
                               {
                                   Work_Id = workApprovals.Work_Id,
                                   Emp_Id = workApprovals.Emp_Id,
                                   Manager_Id = workApprovals.Manager_Id,
                                   Approval_Status = workApprovals.Approval_Status,
                                   Comment = workApprovals.Comment,
                                   Correction = workApprovals.Correction,
                               };

            return await workApproval.ToListAsync();
        }

        public async Task<List<WorkApprovalDTO>> LoadWorkApproval()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var workapp = from workApproval in context.WorkApproval
                          join work in context.Work on workApproval.Work_Id equals work.Work_Id
                          join emp in context.Employee on work.Emp_Id equals emp.EMP_ID
                          where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer") && workApproval.Approval_Status == "Hold"
                          select new WorkApprovalDTO
                          {
                              Work_Id = workApproval.Work_Id,
                              Work_Date = work.Work_Date,
                              Emp_Name = work.Emp_Name,
                              Cust_Name = work.Cust_Name,
                              Proj_Name = work.Proj_Name,
                              Work_Descriptions = work.Work_Descriptions,
                              Approval_Status = workApproval.Approval_Status,
                              Comment = workApproval.Comment,
                              Correction = work.Correction,
                              Approval_Date = workApproval.Approval_Date,
                          };
            return await workapp.ToListAsync();
        }

        public async Task<int> GetHoldWorkCountAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.WorkApproval.CountAsync(w => w.Approval_Status == "Hold");
        }

        public async Task<List<WorkApproval>> GetHoldWorkAsync(int id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.WorkApproval.Where(w => w.Approval_Status == "Hold" && w.Emp_Id == id).ToListAsync();
        }

        public async Task<List<WorkApproval>> GetPendingApprovalForEmployeeAsync(int empId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.WorkApproval
                                 .Where(w => w.Approval_Status == "Hold" && w.Emp_Id == empId)
                                 .ToListAsync();
        }

        public async Task UpdateWorkApprovalAsync(WorkApproval workap)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.WorkApproval.Update(workap);
            await context.SaveChangesAsync();
        }

        // Changed from 'async void' to 'async Task' to prevent crashes and allow awaiting
        public async Task Updateworkinapproval(Work work)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var sql = "update WorkApproval set CORRECTION=@VAL1 WHERE WORK_ID=@VAL2";
            var parameters = new[] {
                new SqlParameter("@VAL1", work.Correction ?? (object)DBNull.Value),
                new SqlParameter("@VAL2", work.Work_Id),
            };
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        //private readonly CustomerDbContext _context;

        //public ApprovalService(CustomerDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task<List<WorkApproval>>? GetWorkApproval()
        //{
        //    var workApproval = from workApprovals in _context.WorkApproval
        //                       join emp in _context.Employee on workApprovals.Emp_Id equals emp.EMP_ID
        //               where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer") && workApprovals.Approval_Status == "Hold"
        //               select new WorkApproval
        //               {
        //                   Work_Id = workApprovals.Work_Id,
        //                   Emp_Id = workApprovals.Emp_Id,
        //                   Manager_Id = workApprovals.Manager_Id,
        //                   Approval_Status = workApprovals.Approval_Status,
        //                   Comment = workApprovals.Comment,
        //                   Correction = workApprovals.Correction,
        //               };

        //    return   await workApproval.ToListAsync();

        //}
        //public async Task<List<WorkApprovalDTO>> LoadWorkApproval()
        //{
        //    var workapp = from workApproval in _context.WorkApproval
        //                  join work in _context.Work on workApproval.Work_Id equals work.Work_Id
        //                  join emp in _context.Employee on work.Emp_Id equals emp.EMP_ID
        //                  where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer") && workApproval.Approval_Status == "Hold"
        //                  select new WorkApprovalDTO
        //                  {
        //                      Work_Id = workApproval.Work_Id,
        //                      Work_Date = work.Work_Date,
        //                      Emp_Name = work.Emp_Name,
        //                      Cust_Name = work.Cust_Name,
        //                      Proj_Name = work.Proj_Name,
        //                      Work_Descriptions = work.Work_Descriptions,
        //                      Approval_Status = workApproval.Approval_Status,
        //                      Comment = workApproval.Comment,
        //                      Correction = work.Correction,
        //                      Approval_Date = workApproval.Approval_Date,

        //                  };
        //    return await workapp.ToListAsync();

        //}

        //public async Task<int> GetHoldWorkCountAsync()
        //{
        //    return await _context.WorkApproval.CountAsync(w => w.Approval_Status == "Hold");
        //}

        //public async Task<List<WorkApproval>> GetHoldWorkAsync(int id)
        //{
        //    return await _context.WorkApproval.Where(w => w.Approval_Status == "Hold" && w.Emp_Id == id).ToListAsync();
        //}

        //public async Task<List<WorkApproval>> GetPendingApprovalForEmployeeAsync(int empId)
        //{
        //    return await _context.WorkApproval
        //                         .Where(w => w.Approval_Status == "Hold" && w.Emp_Id == empId)
        //                         .ToListAsync();
        //}
        //public async Task UpdateWorkApprovalAsync(WorkApproval workap)
        //{
        //    _context.WorkApproval.Update(workap);
        //    await _context.SaveChangesAsync();

        //}
        //public async void Updateworkinapproval(Work work)
        //{
        //    var sql = "update WorkApproval set CORRECTION=@VAL1 WHERE WORK_ID=@VAL2";
        //    var parameters = new[] {
        //    new SqlParameter("@VAL1",work.Correction),
        //    new SqlParameter("@VAL2",work.Work_Id),
        //    };
        //    await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        //    await _context.SaveChangesAsync();

        //}
    }
}
