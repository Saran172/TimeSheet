using DevExpress.Blazor;
using System.Data;
using System.Data.SqlClient;
using Task_1.Entities;

namespace Task_1.Report.EmployeeReport
{
    public partial class EmployeeReport
    {
        IGrid Grid { get; set; }
        // IEnumerable<Product> Products { get; set; }
        // IEnumerable<Category> Categories { get; set; }
        IEnumerable<Employee> employees { get; set; } = new List<Employee>();
        Employee emps { get; set; }
        private bool isLoading = true;
        Employee SelectedCategories { get; set; }
        Employee emp = new Employee();
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);
            isLoading = false;
            await getdata();
            // Products = await GetProductsAsync();

        }


        // RenderFragment GetSelectedItemDescription()
        // {
        //     if (SelectedCategories != null)
        //     {
        //         return @<text>
        //     Selected Item: (
        //     @GetFieldDescription(nameof(Employee.EMP_NAME), SelectedCategories.EMP_NAME),

        //     @GetFieldDescription(nameof(Employee.EMP_DESIGNATION), SelectedCategories.EMP_DESIGNATION)
        //     )
        // </text>;
        //     }
        //     return @<text>Selected Item: <b>null</b></text>;


        // }
        // RenderFragment GetFieldDescription(string fieldName, object value)
        // {
        //     return @<text>@fieldName: <b>@value</b></text>;
        // }

        private async Task getdata()

        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = " SELECT* FROM Employee";

                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    employees = await _data.ConvertToList<Employee>(dataTable);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
        private void NavigateToWorkDetails(int empId)
        {
            NavigationManager.NavigateTo($"/employeeRep/{empId}");
        }

    }
}
