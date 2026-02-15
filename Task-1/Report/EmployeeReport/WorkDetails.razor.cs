using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Data.SqlClient;
using Task_1.Entities;

namespace Task_1.Report.EmployeeReport
{
    public partial class WorkDetails
    {
        IGrid Grid;
        [Parameter]
        public int id { get; set; }
        object GridData { get; set; }
        const string ExportFileName = "ExportResult";
        IReadOnlyList<Work> Categories { get; set; }
        List<Workdto> work = new List<Workdto>();
        List<Workdto> working = new List<Workdto>();
        DateTime DateTimeValue { get; set; }
        DateTime DateTimeStart { get; set; }
        DateTime DateTimeEnd { get; set; }
        protected override async Task OnInitializedAsync()
        {
            // Categories = (await workService.GetWorkEditableAsync()).ToList();
            // work = await workService.Getbyid(id);
            await getdata();
            DateTimeStart = DateTime.Today.AddDays(-1);
            DateTimeEnd = DateTime.Today;
            await UpdateGrid(DateTimeStart, DateTimeEnd);
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
        private async Task getdata()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = " SELECT * FROM Work where EMP_ID =@empid";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@empid", id);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    work = await _data.ConvertToList<Workdto>(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
        private async Task UpdateGrid(DateTime date1, DateTime date2)
        {


            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "  SELECT* FROM Work where WORK_DATE BETWEEN CONVERT(DATE, @date1, 120) AND CONVERT(DATE, @date2, 120)  and EMP_ID=@id";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@date1", date1);
                    command.Parameters.AddWithValue("@date2", date2);
                    command.Parameters.AddWithValue("@id", id);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    work = await _data.ConvertToList<Workdto>(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
        private async Task GetDetails(int pid)
        {

            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = " SELECT EMP_ID,START_TIME,END_TIME,TOTAL_HOURS FROM Work where PROJ_ID =@projid";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projid", pid);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    working = await _data.ConvertToList<Workdto>(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}
