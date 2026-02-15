using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using System.Data;
using System.Data.SqlClient;
using Task_1.Entities;

namespace Task_1.Report.DepartmentReport
{
    public partial class DepartmentReport
    {
        IGrid Grid { get; set; }
        const string ExportFileName = "ExportResult";
        // IEnumerable<Product> Products { get; set; }
        // IEnumerable<Category> Categories { get; set; }
        IEnumerable<Employee> employee { get; set; }
        IEnumerable<Employee> SelectedCategories { get; set; }

        public IEnumerable<Employee> FilteredEmployees
        {
            get
            {

                return employee
                    .GroupBy(emp => emp.EMP_DESIGNATION)
                    .Select(group => group.First())
                    .ToList();
            }
        }


        private bool isLoading = true;
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);
            isLoading = false;
            await getdata();
            // Products = await GetProductsAsync();
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
        void TagBox_ValuesChanged(IEnumerable<Employee> newSelectedCategories)
        {
            SelectedCategories = newSelectedCategories;
            var filterCriteria = SelectedCategories.Count() > 0
            ? new InOperator("EMP_DESIGNATION", SelectedCategories.Select(c => c.EMP_DESIGNATION))
            : null;
            Grid.SetFieldFilterCriteria("EMP_DESIGNATION", filterCriteria);
        }

        private async Task getdata()

        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Employee";

                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    employee = await _data.ConvertToList<Employee>(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }



        // private async Task getDep()

        // {
        //     try
        //     {
        //         string connectionString = Configuration.GetConnectionString("DefaultConnection");

        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             string query = "select distinct EMP_DESIGNATION from Employee ";

        //             SqlCommand command = new SqlCommand(query, connection);
        //             DataTable dataTable = new DataTable();

        //             using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //             {
        //                 adapter.Fill(dataTable);
        //             }
        //             employee = await _data.ConvertToList<Employee>(dataTable);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("Error: " + ex.Message);
        //     }

        // }
    }
}
