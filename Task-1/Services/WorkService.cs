using DevExpress.Data.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Task_1.Interface;
using Task_1.DbCon;
using Task_1.Entities;

namespace Task_1.Services
{
    public class WorkService : IWorkService
    {
        private readonly IDbContextFactory<CustomerDbContext> _contextFactory;

        public WorkService(IDbContextFactory<CustomerDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<List<Work>> workToday(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work
                .Where(e => e.Work_Date == DateTime.Today && e.Emp_Id == id)
                .ToListAsync();
        }

        public async Task Updateworkinapproval(Work work)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var sql = "UPDATE WorkApproval SET MANAGER_ID=@VAL1, APPROVAL_STATUS=@VAL2, CORRECTION=@VAL7 WHERE WORK_ID=@VAL4";
            var parameters = new[] {
                new SqlParameter("@VAL1", work.Manager_Id),
                new SqlParameter("@VAL2", work.Manager_Approval ?? (object)DBNull.Value),
                new SqlParameter("@VAL7", work.Correction ?? (object)DBNull.Value),
                new SqlParameter("@VAL4", work.Work_Id),
            };
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
            // SaveChangesAsync is not needed for ExecuteSqlRawAsync, but it doesn't hurt.
        }

        public async Task Updatework(Work work)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var sql = "UPDATE Work SET CORRECTION=@VAL1 WHERE WORK_ID=@VAL2";
            var parameters = new[] {
                new SqlParameter("@VAL1", work.Correction ?? (object)DBNull.Value),
                new SqlParameter("@VAL2", work.Work_Id),
            };
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task Createworkinapproval(Work work)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var sql = "INSERT INTO WorkApproval(WORK_ID, EMP_ID, MANAGER_ID, APPROVAL_STATUS, COMMENT, APPROVAL_DATE, CORRECTION) VALUES (@val1, @val2, @val3, @val4, @val5, @val6, @val7)";
            var parameters = new[] {
                new SqlParameter("@val1", work.Work_Id),
                new SqlParameter("@val2", work.Emp_Id),
                new SqlParameter("@val3", work.Manager_Id),
                new SqlParameter("@val4", work.Manager_Approval ?? (object)DBNull.Value),
                new SqlParameter("@val5", work.Comment ?? (object)DBNull.Value),
                new SqlParameter("@val7", work.Correction ?? (object)DBNull.Value),
                new SqlParameter("@val6", DateTime.Today),
            };
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<bool> WorkDateNull(DateTime date, int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work.AnyAsync(e => e.Work_Date == date && e.Emp_Id == id);
        }

        public async Task<List<Work>> GetWorkEditableAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = from works in context.Work
                        join emp in context.Employee on works.Emp_Id equals emp.EMP_ID
                        where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer" || emp.EMP_DESIGNATION == "Flutter Developer" || emp.EMP_DESIGNATION == "SAP" || emp.EMP_DESIGNATION == "Testing & Support" || emp.EMP_DESIGNATION == "Social Media Marketing Executive") && works.Manager_Approval == null
                        select new Work
                        {
                            Work_Id = works.Work_Id,
                            Work_Date = works.Work_Date,
                            Emp_Name = works.Emp_Name,
                            Emp_Id = works.Emp_Id,
                            Cust_Name = works.Cust_Name,
                            Proj_Name = works.Proj_Name,
                            Ticket_Number = works.Ticket_Number,
                            Work_Descriptions = works.Work_Descriptions,
                            Start_Time = works.Start_Time,
                            End_Time = works.End_Time,
                            Total_Hours = works.Total_Hours,
                            Manager_Id = works.Manager_Id,
                            Manager_Name = works.Manager_Name,
                            Manager_Approval = works.Manager_Approval,
                            Comment = works.Comment,
                            Correction = works.Correction,
                        };

            return await query.ToListAsync();
        }

        public async Task<List<Work>> GetWorkbydateid(DateTime date, int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work.Where(e => e.Work_Date == date && e.Emp_Id == id).ToListAsync();
        }

        public async Task<List<Work>> GetWorkbybetweendates(DateTime date1, DateTime date2, int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work.Where(e => e.Work_Date >= date1 && e.Work_Date <= date2 && e.Emp_Id == id).ToListAsync();
        }

        public async Task InsertWorkAsync(Work newDataItem)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                await context.Work.AddAsync(newDataItem);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public async Task RemoveWorkAsync(int dataId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var find = await context.Work.FindAsync(dataId);
            if (find != null)
            {
                context.Work.Remove(find);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateWorkAsync(Work dataItem, IDictionary<string, object> newValues)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Attach the item so the new context knows about it
            context.Work.Attach(dataItem);

            foreach (var val in newValues)
            {
                var propertyInfo = dataItem.GetType().GetProperty(val.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(dataItem, Convert.ChangeType(val.Value, propertyInfo.PropertyType));
                }
            }

            context.Work.Update(dataItem);
            await context.SaveChangesAsync();
        }

        public async Task UpdateWorkAsync(Work work)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Work.Update(work);
            await context.SaveChangesAsync();
        }

        public async Task<int> GetPendingWorkCountAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work.CountAsync(w => w.Manager_Approval == null);
        }

        public async Task<List<Work>> GetWorkToday(int employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Work
                .Where(e => e.Work_Date == DateTime.Today && e.Emp_Id == employeeId)
                .ToListAsync();
        }
        //private readonly IDbContextFactory<CustomerDbContext> _contextFactory;

        //public WorkService(IDbContextFactory<CustomerDbContext> context)
        //{
        //    _contextFactory = context;
        //}

        //public async Task<List<Work>> workToday(int id)
        //{
        //    var work = await _contextFactory.Work.Where(e => e.Work_Date == DateTime.Today && e.Emp_Id == id).ToListAsync();
        //    return work;
        //}

        //public async Task Updateworkinapproval(Work work)
        //{
        //    var sql = "update WorkApproval set MANAGER_ID=@VAL1,APPROVAL_STATUS=@VAL2,CORRECTION=@VAL7 WHERE WORK_ID=@VAL4";
        //    var parameters = new[] { 
        //    new SqlParameter("@VAL1",work.Manager_Id),
        //    new SqlParameter("@VAL2",work.Manager_Approval),
        //    new SqlParameter("@VAL7",work.Correction?? (object)DBNull.Value),
        //    new SqlParameter("@VAL4",work.Work_Id),
        //    };
        //    await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        //    await _context.SaveChangesAsync();

        //}

        //public async Task Updatework(Work work)
        //{
        //    var sql = "update Work set CORRECTION=@VAL1 WHERE WORK_ID=@VAL2";
        //    var parameters = new[] {
        //    new SqlParameter("@VAL1",work.Correction),
        //    new SqlParameter("@VAL2",work.Work_Id),
        //    };
        //    await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        //    await _context.SaveChangesAsync();

        //}
        //public async Task Createworkinapproval(Work work)
        //{
        //    var sql = "INSERT INTO WorkApproval(WORK_ID,EMP_ID,MANAGER_ID,APPROVAL_STATUS,COMMENT,APPROVAL_DATE,CORRECTION) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7)";
        //    var parameters = new[] {
        //    new SqlParameter("@val1", work.Work_Id),
        //    new SqlParameter("@val2", work.Emp_Id),
        //    new SqlParameter("@val3", work.Manager_Id),
        //    new SqlParameter("@val4",work.Manager_Approval?? (object)DBNull.Value),
        //    new SqlParameter("@val5",work.Comment?? (object)DBNull.Value),
        //    new SqlParameter("@val7",work.Correction?? (object)DBNull.Value),
        //    new SqlParameter("@val6",DateTime.Today),

        //};
        //    await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        //    await _context.SaveChangesAsync();

        //}

        //public async Task<bool> WorkDateNull(DateTime date, int id)
        //{
        //    var work = await _context.Work.AnyAsync(e => e.Work_Date == date && e.Emp_Id == id);
        //    return work;
        //}
        //public async Task<List<Work>> GetWorkEditableAsync()
        //{
        //    var work = from works in _context.Work
        //               join emp in _context.Employee on works.Emp_Id equals emp.EMP_ID
        //               where (emp.EMP_DESIGNATION == "Trainee" || emp.EMP_DESIGNATION == "Dot Net Developer" || emp.EMP_DESIGNATION == "Flutter Developer" || emp.EMP_DESIGNATION == "SAP" || emp.EMP_DESIGNATION == "Testing & Support" || emp.EMP_DESIGNATION == "Social Media Marketing Executive") && works.Manager_Approval == null
        //               select new Work
        //               {
        //                   Work_Id = works.Work_Id,
        //                   Work_Date = works.Work_Date,
        //                   Emp_Name = works.Emp_Name,
        //                   Emp_Id = works.Emp_Id,
        //                   Cust_Name = works.Cust_Name,
        //                   Proj_Name = works.Proj_Name,
        //                   Ticket_Number = works.Ticket_Number,
        //                   Work_Descriptions = works.Work_Descriptions,
        //                   Start_Time = works.Start_Time,
        //                   End_Time = works.End_Time,
        //                   Total_Hours = works.Total_Hours,
        //                   Manager_Id = works.Manager_Id,
        //                   Manager_Name = works.Manager_Name,
        //                   Manager_Approval = works.Manager_Approval,
        //                   Comment = works.Comment,
        //                   Correction = works.Correction,
        //               };

        //    return await work.ToListAsync();
        //    //var work = await _context.Work.ToListAsync();
        //    //return work;
        //}

        //public async Task<List<Work>> GetWorkbydateid(DateTime date, int id)
        //{
        //    var works = await _context.Work.Where(e => e.Work_Date == date && e.Emp_Id == id).ToListAsync();
        //    return works;

        //}
        //public async Task<List<Work>> GetWorkbybetweendates(DateTime date1, DateTime date2, int id)
        //{
        //    var works = await _context.Work.Where(e => e.Work_Date >= date1 && e.Work_Date <= date2 && e.Emp_Id ==id).ToListAsync();
        //    return works;
        //}
        public async Task<List<Work>> GetWorkbydate(DateTime date1, DateTime date2)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Work
                .Where(w => w.Work_Date >= date1 && w.Work_Date <= date2)
                .ToListAsync();
        }

        public async Task<List<Work>> GetWorkByEmpid(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Work
                .Where(w => w.Emp_Id == id)
                .ToListAsync();
        }

        public async Task<List<Work>> GetWorkbydesignationandApproval()
        {
            using var context = _contextFactory.CreateDbContext();

            var work = from works in context.Work
                       join emp in context.Employee
                           on works.Emp_Id equals emp.EMP_ID
                       where
                           (
                               emp.EMP_DESIGNATION == "Trainee"
                               || emp.EMP_DESIGNATION == "Dot Net Developer"
                               || emp.EMP_DESIGNATION == "Flutter Developer"
                               || emp.Reporting_Manager == "Ramesh"
                           )
                           && works.Manager_Approval == null
                       select new Work
                       {
                           Work_Id = works.Work_Id,
                           Work_Date = works.Work_Date,
                           Emp_Name = works.Emp_Name,
                           Emp_Id = works.Emp_Id,
                           Cust_Name = works.Cust_Name,
                           Proj_Name = works.Proj_Name,
                           Ticket_Number = works.Ticket_Number,
                           Work_Descriptions = works.Work_Descriptions,
                           Start_Time = works.Start_Time,
                           End_Time = works.End_Time,
                           Total_Hours = works.Total_Hours,
                           Manager_Id = works.Manager_Id,
                           Manager_Name = works.Manager_Name,
                           Manager_Approval = works.Manager_Approval,
                           Comment = works.Comment,
                           Correction = works.Correction,
                       };

            return await work.ToListAsync();
        }

        public async Task<List<Work>> GetWorkbytestdesignationandApproval()
        {
            using var context = _contextFactory.CreateDbContext();

            var work = from works in context.Work
                       join emp in context.Employee
                           on works.Emp_Id equals emp.EMP_ID
                       where
                           (
                               emp.EMP_DESIGNATION == "SAP"
                               || emp.EMP_DESIGNATION == "Testing & Support"
                               || emp.EMP_DESIGNATION == "Social Media Marketing Executive"
                               || emp.EMP_DESIGNATION == "Report Developer"
                               || emp.Reporting_Manager == "Sindhu"
                           )
                           && works.Manager_Approval == null
                       select new Work
                       {
                           Work_Id = works.Work_Id,
                           Work_Date = works.Work_Date,
                           Emp_Name = works.Emp_Name,
                           Emp_Id = works.Emp_Id,
                           Cust_Name = works.Cust_Name,
                           Proj_Name = works.Proj_Name,
                           Ticket_Number = works.Ticket_Number,
                           Work_Descriptions = works.Work_Descriptions,
                           Start_Time = works.Start_Time,
                           End_Time = works.End_Time,
                           Total_Hours = works.Total_Hours,
                           Manager_Id = works.Manager_Id,
                           Manager_Name = works.Manager_Name,
                           Manager_Approval = works.Manager_Approval,
                           Comment = works.Comment,
                           Correction = works.Correction,
                       };

            return await work.ToListAsync();
        }

        public async Task<List<Work>> GetWorkbydateAndDesignation(DateTime date1, DateTime date2)
        {
            using var context = _contextFactory.CreateDbContext();

            var work = from works in context.Work
                       join emp in context.Employee
                           on works.Emp_Id equals emp.EMP_ID
                       where
                           (
                               emp.EMP_DESIGNATION == "Trainee"
                               || emp.EMP_DESIGNATION == "Dot Net Developer"
                               || emp.EMP_DESIGNATION == "Flutter Developer"
                               || emp.Reporting_Manager == "Ramesh"
                           )
                           && works.Work_Date >= date1
                           && works.Work_Date <= date2
                       select new Work
                       {
                           Work_Id = works.Work_Id,
                           Work_Date = works.Work_Date,
                           Emp_Name = works.Emp_Name,
                           Emp_Id = works.Emp_Id,
                           Cust_Name = works.Cust_Name,
                           Proj_Name = works.Proj_Name,
                           Ticket_Number = works.Ticket_Number,
                           Work_Descriptions = works.Work_Descriptions,
                           Start_Time = works.Start_Time,
                           End_Time = works.End_Time,
                           Total_Hours = works.Total_Hours,
                           Manager_Id = works.Manager_Id,
                           Manager_Name = works.Manager_Name,
                           Manager_Approval = works.Manager_Approval,
                           Comment = works.Comment,
                           Correction = works.Correction,
                       };

            return await work.ToListAsync();
        }

        public async Task<List<Work>> GetWorkbydateAndDesignation1(DateTime date1, DateTime date2)
        {
            using var context = _contextFactory.CreateDbContext();

            var work = from works in context.Work
                       join emp in context.Employee
                           on works.Emp_Id equals emp.EMP_ID
                       where
                           (
                               emp.EMP_DESIGNATION == "SAP"
                               || emp.EMP_DESIGNATION == "Testing & Support"
                               || emp.EMP_DESIGNATION == "Social Media Marketing Executive"
                               || emp.EMP_DESIGNATION == "Report Developer"
                               || emp.Reporting_Manager == "Sindhu"
                           )
                           && works.Work_Date >= date1
                           && works.Work_Date <= date2
                       select new Work
                       {
                           Work_Id = works.Work_Id,
                           Work_Date = works.Work_Date,
                           Emp_Name = works.Emp_Name,
                           Emp_Id = works.Emp_Id,
                           Cust_Name = works.Cust_Name,
                           Proj_Name = works.Proj_Name,
                           Ticket_Number = works.Ticket_Number,
                           Work_Descriptions = works.Work_Descriptions,
                           Start_Time = works.Start_Time,
                           End_Time = works.End_Time,
                           Total_Hours = works.Total_Hours,
                           Manager_Id = works.Manager_Id,
                           Manager_Name = works.Manager_Name,
                           Manager_Approval = works.Manager_Approval,
                           Comment = works.Comment,
                           Correction = works.Correction,
                       };

            return await work.ToListAsync();
        }


        ////public async IActionResult Getbyid(int id)
        ////{
        ////   var  work =await _context.Work.FindAsync(id);
        ////    return work;
        ////}
        //public async Task InsertWorkAsync(IDictionary<string, object> newValues)
        //{
        //    var work = new Work();

        //    foreach (var emp in newValues)
        //    {
        //        var propertyInfo = work.GetType().GetProperty(emp.Key);
        //        if (propertyInfo != null)
        //        {
        //            propertyInfo.SetValue(work, Convert.ChangeType(emp.Value, propertyInfo.PropertyType));
        //        }
        //    }
        //    var wor = await _context.Work.AddAsync(work);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task InsertWorkAsync(Work newDataItem)
        //{
        //    try
        //    {
        //        await _context.Work.AddAsync(newDataItem);
        //        await _context.SaveChangesAsync();           
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //public async Task RemoveWorkAsync(int dataItem)
        //{
        //    var find = await _context.Work.FindAsync(dataItem);
        //    _context.Work.Remove(find);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task UpdateWorkAsync(Work dataItem, IDictionary<string, object> newValues)
        //{
        //    foreach (var emp in newValues)
        //    {
        //        var propertyInfo = dataItem.GetType().GetProperty(emp.Key);
        //        if (propertyInfo != null)
        //        {
        //            propertyInfo.SetValue(dataItem, Convert.ChangeType(emp.Value, propertyInfo.PropertyType));
        //        }
        //    }

        //    _context.Work.Update(dataItem);
        //    await _context.SaveChangesAsync();

        //}
        //public async Task UpdateWorkAsync(Work work)
        //{
        //    _context.Work.Update(work);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<int> GetPendingWorkCountAsync()
        //{
        //    return await _context.Work.CountAsync(w => w.Manager_Approval == null);
        //}
        //public Task<List<Work>> GetWorkToday(int employeeId)
        //{
        //    var work = _context.Work.Where(e => e.Work_Date == DateTime.Today && e.Emp_Id == employeeId).ToListAsync();
        //    return work;
        //}
    }
}

