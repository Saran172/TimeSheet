using DevExpress.XtraPrinting.Native;
using Microsoft.EntityFrameworkCore;
using Task_1.DbCon;
using Task_1.Entities;

namespace Task_1.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<CustomerDbContext> _dbFactory;

        public ProjectService(IDbContextFactory<CustomerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Project>> GetProjectEditableAsync(CancellationToken ct = default)
        {
            using var context = await _dbFactory.CreateDbContextAsync(ct);
            // Added the CancellationToken to the ToListAsync call
            return await context.Project.ToListAsync(ct);
        }

        public async Task InsertProjectAsync(Project project)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            try
            {
                var existingProject = await context.Project.FindAsync(project.Proj_Id);

                if (existingProject != null)
                {
                    context.Entry(existingProject).CurrentValues.SetValues(project);
                }
                else
                {
                    await context.Project.AddAsync(project);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Logic for handling errors could go here
            }
        }

        public async Task RemoveProjectAsync(int dataItem)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var find = await context.Project.FindAsync(dataItem);
            if (find != null)
            {
                context.Project.Remove(find);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateProjectAsync(Project dataItem, IDictionary<string, object> newValues)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            // We must attach the item to the new context instance to track changes
            context.Project.Attach(dataItem);

            foreach (var emp in newValues)
            {
                var propertyInfo = dataItem.GetType().GetProperty(emp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(dataItem, Convert.ChangeType(emp.Value, propertyInfo.PropertyType));
                }
            }

            context.Project.Update(dataItem);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.Project.Update(project);
            await context.SaveChangesAsync();
        }
        //private readonly CustomerDbContext _context;

        //public ProjectService(CustomerDbContext context)
        //{
        //    _context = context;
        //}
        //public async Task<List<Project>> GetProjectEditableAsync(CancellationToken ct = default)
        //{
        //    var project = await _context.Project.ToListAsync();
        //    return project;
        //}

        //public async Task InsertProjectAsync(Project project)
        //{
        //    try
        //    {
        //        var existingProject = await _context.Project
        //                .FindAsync(project.Proj_Id);

        //        if (existingProject != null)
        //        {
        //            _context.Entry(existingProject).CurrentValues.SetValues(project);
        //        }
        //        else
        //        {
        //            _context.Project.Add(project);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {}


        //}
        //public async Task RemoveProjectAsync(int dataItem)
        //{
        //    var find = await _context.Project.FindAsync(dataItem);
        //    _context.Project.Remove(find);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task UpdateProjectAsync(Project dataItem, IDictionary<string, object> newValues)
        //{
        //    foreach (var emp in newValues)
        //    {
        //        var propertyInfo = dataItem.GetType().GetProperty(emp.Key);
        //        if (propertyInfo != null)
        //        {
        //            propertyInfo.SetValue(dataItem, Convert.ChangeType(emp.Value, propertyInfo.PropertyType));
        //        }
        //    }

        //    _context.Project.Update(dataItem);
        //    await _context.SaveChangesAsync();

        //}
        //public async Task UpdateProjectAsync(Project project)
        //{
        //    _context.Project.Update(project);
        //    await _context.SaveChangesAsync();

        //}
    }
}
