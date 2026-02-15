using DevExpress.Blazor;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Data;
using System.Data.SqlClient;
using Task_1.Entities;

namespace Task_1.Report.DailySheet_Report
{
    public partial class DailySheet
    {
        IGrid Grid { get; set; }
        const string ExportFileName = "ExportResult";
        object GridData { get; set; }
        IReadOnlyList<Work> Categories { get; set; }
        List<WorkProjdto> work = new List<WorkProjdto>();
        IQueryable<WorkProjdto> work1;
        List<Workdto> working = new List<Workdto>();
        DateTime DateTimeValue { get; set; }
        DateTime DateTimeStart { get; set; }
        DateTime DateTimeEnd { get; set; }
        private bool isLoading = true;

        private bool isManager;
        private bool isEditable = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await CustomAuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    isManager = user.IsInRole("manager");
                }

                isLoading = false;
                await getdata();
                DateTimeStart = DateTime.Today.AddDays(-1);
                DateTimeEnd = DateTime.Today;
                await UpdateGrid(DateTimeStart, DateTimeEnd);
            }
            catch (Exception ex)
            {
                await Jsruntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }
        async Task ExportXlsxItem_Click()
        {
            await Grid.ExportToXlsxAsync(ExportFileName);
        }
        async Task ExportXlsItem_Click()
        {
            await Grid.ExportToXlsAsync(ExportFileName);
        }
        async Task ExportCsvItem_Click()
        {
            await Grid.ExportToCsvAsync(ExportFileName);
        }

        private void ToggleEditability()
        {
            EditStateService.ToggleEditability();
        }

        private async Task getdata()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // string query = "WITH CTE AS (SELECT w.Work_Id, w.WORK_DATE, w.CREATED_DATETIME, w.Emp_Name, w.WORK_DESCRIPTIONS, w.START_TIME, w.END_TIME, w.TOTAL_HOURS, w.Proj_Name, w.CUST_NAME, p.So_Number, p.CUST_CODE, p.IsBillable, ROW_NUMBER() OVER (PARTITION BY w.Work_Id ORDER BY w.Work_Id, w.WORK_DATE DESC) AS RowNum FROM Work w JOIN Project p  ON w.Proj_Name = p.Project_Name) SELECT Work_Id, WORK_DATE, CREATED_DATETIME, Emp_Name, WORK_DESCRIPTIONS, START_TIME,END_TIME,TOTAL_HOURS, Proj_Name, CUST_NAME, So_Number, CUST_CODE, IsBillable FROM CTE where RowNum =  1";

                    string query = "select * from Work";
                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    work = await _data.ConvertToList<WorkProjdto>(dataTable);

                }

                var projects = await (
                                from p in _context.Project
                                join c in _context.Customer on p.Cust_Code equals c.Cust_Code
                                select new
                                {
                                    p.Proj_Id,
                                    p.Project_Name,
                                    p.Cust_Code,
                                    Cust_Name = c.Cust_Name,
                                    p.So_Number,
                                    p.IsBillable
                                }
                            ).ToListAsync();

                foreach (var w in work)
                {
                    var project = projects.FirstOrDefault(p => p.Project_Name == w.PROJ_NAME && p.Cust_Name == w.CUST_NAME);

                    if (project != null)
                    {
                        w.So_Number = project.So_Number;
                        w.proj_Id = project.Proj_Id;
                        w.CUST_CODE = project.Cust_Code;
                        w.IsBillable = project.IsBillable;
                        w.TaskStatus = "Project";
                    }
                    else if (!string.IsNullOrWhiteSpace(w.TICKET_NUMBER) && !string.IsNullOrWhiteSpace(w.CUST_NAME))
                    {
                        w.TaskStatus = "Ticket";
                    }
                    else
                    {
                        w.TaskStatus = "No Task";
                    }
                }

                //work = work.ToList();

            }
            catch (Exception ex)
            {
                await Jsruntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }
        private async Task UpdateGrid(DateTime date1, DateTime date2)
        {


            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // string query = "WITH CTE AS (SELECT w.Work_Id, w.WORK_DATE, w.CREATED_DATETIME, w.Emp_Name, w.WORK_DESCRIPTIONS, w.START_TIME, w.END_TIME, w.TOTAL_HOURS, w.Proj_Name, w.CUST_NAME, p.So_Number, p.CUST_CODE, p.IsBillable, ROW_NUMBER() OVER (PARTITION BY w.Work_Id ORDER BY w.Work_Id, w.WORK_DATE DESC) AS RowNum FROM Work w JOIN Project p  ON w.Proj_Name = p.Project_Name) SELECT Work_Id, WORK_DATE, CREATED_DATETIME, Emp_Name, WORK_DESCRIPTIONS, START_TIME,END_TIME,TOTAL_HOURS, Proj_Name, CUST_NAME, So_Number, CUST_CODE, IsBillable FROM CTE where WORK_DATE BETWEEN CONVERT(DATE, @date1, 120) AND CONVERT(DATE, @date2, 120) AND RowNum =  1";

                    string query = "select * from Work where WORK_DATE BETWEEN CONVERT(DATE, @date1, 120) AND CONVERT(DATE, @date2, 120)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@date1", date1);
                    command.Parameters.AddWithValue("@date2", date2);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    work = await _data.ConvertToList<WorkProjdto>(dataTable);
                }

                var projects = await (
                               from p in _context.Project
                               join c in _context.Customer on p.Cust_Code equals c.Cust_Code
                               select new
                               {
                                   p.Proj_Id,
                                   p.Project_Name,
                                   p.Cust_Code,
                                   Cust_Name = c.Cust_Name,
                                   p.So_Number,
                                   p.IsBillable
                               }
                           ).ToListAsync();

                foreach (var w in work)
                {
                    var project = projects.FirstOrDefault(p => p.Project_Name == w.PROJ_NAME && p.Cust_Name == w.CUST_NAME);

                    if (project != null)
                    {
                        w.So_Number = project.So_Number;
                        w.proj_Id = project.Proj_Id;
                        w.CUST_CODE = project.Cust_Code;
                        w.IsBillable = project.IsBillable;
                        w.TaskStatus = "Project";
                    }
                    else if (!string.IsNullOrWhiteSpace(w.TICKET_NUMBER) && !string.IsNullOrWhiteSpace(w.CUST_NAME))
                    {
                        w.TaskStatus = "Ticket";
                    }
                    else
                    {
                        w.TaskStatus = "No Task";
                    }
                }

            }
            catch (Exception ex)
            {
                await Jsruntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }

    }
}
