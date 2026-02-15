using DevExpress.Blazor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Data;
using Microsoft.Data.SqlClient;
using Task_1.Model;
using Task_1.Entities;

namespace Task_1.Pages.EmployeeScreen
{
    public partial class EmployeeMaster
    {
        bool EditItemsEnabled { get; set; }
        int FocusedRowVisibleIndex { get; set; }
        Employee[] Data { get; set; }
        private bool isLoading = true;
        IGrid Grid { get; set; }
        public int action { get; set; }
        private List<Employee> lt_employee = new List<Employee>();
        Employee employee = new Employee();

       

        List<DesignationDto> lt_designations = new List<DesignationDto>();


        public class ReportingTo()
        {
            public int EMP_ID { get; set; }
            public string EMP_NAME { get; set; }
        }

        List<ReportingTo> lt_reportingmanagers = new List<ReportingTo>();

        public int EMP_ID { get; set; }
        public string EMP_CODE { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_CONTACTNO { get; set; }
        public string EMP_CONTACTMAIL { get; set; }
        public string Role { get; set; }
        public string Reporting_Manager { get; set; }
        public string EMP_DESIGNATION { get; set; }
        public string? EMP_STATUS { get; set; }
        public string EMP_PASSWORD { get; set; }

        private int? itemToDelete;

        public bool showAddDesignationModal { get; set; } = false;
        public string newDesignation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = false;
                await LoadGridDataAsync();
                await LoadDesignations();
                await LoadReportingManagers();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            
        }

        void OpenAddDesignationModal()
        {
            newDesignation = string.Empty;
            showAddDesignationModal = true;
        }
        void CloseAddDesignationModal()
        {
            newDesignation = string.Empty;
            showAddDesignationModal = false;
        }

        async Task AddDesignation()
        {
            try
            {
                var trimmedDesignation = newDesignation?.Trim();

                var sql = @"IF NOT EXISTS (SELECT 1 FROM Designations WHERE LOWER(EmployeeDesignations) = LOWER(@val1))
                            BEGIN
                                INSERT INTO Designations (EmployeeDesignations) VALUES (@val1);
                            END
                            ELSE
                            BEGIN
                                SELECT -1; 
                            END";

                var parameters = new[]
                {
                     new SqlParameter("@val1", trimmedDesignation)
                };

                int result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                if (result <= 0 )
                {
                    await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Duplicate", "This designation already exists!");
                    return;
                }

                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Designation added successfully!");
                CloseAddDesignationModal();
                await LoadDesignations();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            StateHasChanged();  
        }

        async Task LoadGridDataAsync()
        {
            try
            {
                var data = await employeeservice.GetEmployeeEditableAsync();
                Data = data.ToArray();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }

        public async Task LoadDesignations()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "Select Distinct * from Designations";

                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    var designations = await _data.ConvertToList<DesignationDto>(dataTable);
                    lt_designations = designations.OrderBy(x => x.EmployeeDesignations).ToList();
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }
        public async Task LoadReportingManagers()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "Select Distinct * from Employee where Role = 'manager' and EMP_STATUS = 'Active'";

                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    lt_reportingmanagers = await _data.ConvertToList<ReportingTo>(dataTable);
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }

        protected async void AddModel()
        {

            action = 0;
            lt_employee.Clear();
            employee = new Employee();
            StateHasChanged();
        }
        void Grid_FocusedRowChanged(GridFocusedRowChangedEventArgs args)
        {
            FocusedRowVisibleIndex = args.VisibleIndex;
            UpdateEditItemsEnabled(true);
        }
        void UpdateEditItemsEnabled(bool enabled)
        {
            EditItemsEnabled = enabled;
        }
        void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var newEmployee = (Employee)e.EditModel;
                newEmployee.EMP_NAME = "";

            }
        }
        async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            // await customerservice.RemoveCustomerAsync((Customer)e.DataItem);
            await LoadGridDataAsync();
            if (Data.Length == 0)
                UpdateEditItemsEnabled(false);
        }
        async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                await employeeservice.InsertEmployeeAsync((Employee)e.EditModel);
                UpdateEditItemsEnabled(true);
            }
            else
                // await customerservice.UpdateCustomerAsync((Customer)e.DataItem, (Customer)e.EditModel);
                await LoadGridDataAsync();
        }
        async Task NewItem_Click()
        {
            await Grid.StartEditNewRowAsync();
        }
        async Task EditItem_Click()
        {
            await Grid.StartEditRowAsync(FocusedRowVisibleIndex);
        }
        void DeleteItem_Click()
        {
            Grid.ShowRowDeleteConfirmation(FocusedRowVisibleIndex);
        }
        void ColumnChooserItem_Click(ToolbarItemClickEventArgs e)
        {
            Grid.ShowColumnChooser("");
        }
        void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow && e.VisibleIndex % 2 == 1)
            {
                e.CssClass = "alt-item";
            }
            if (e.ElementType == GridElementType.HeaderCell)
            {
                e.Style = "background-color: rgba(0, 0, 0, 0.08)";
                e.CssClass = "header-bold";
            }
        }

        async Task AddItem()
        {
            ValidateForm();

            if (IsValid)
            {
                var passwordHasher = new PasswordHasher<string>();
                employee.EMP_PASSWORD = passwordHasher.HashPassword(null, employee.EMP_PASSWORD);
                await employeeservice.InsertEmployeeAsync(employee);
                await OnInitializedAsync();
                StateHasChanged();
                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Added successfully");
            }
            else
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter required field!");
            }

        }
        private Dictionary<string, string> validationErrors = new();

        private void ValidateForm()
        {
            validationErrors.Clear();

            if (string.IsNullOrWhiteSpace(employee.EMP_CODE))
                validationErrors["EMP_CODE"] = "EMP_CODE is required.";
            if (string.IsNullOrWhiteSpace(employee.EMP_NAME))
                validationErrors["EMP_NAME"] = "EMP_NAME is required.";
            if (string.IsNullOrWhiteSpace(employee.EMP_CONTACTNO))
                validationErrors["EMP_CONTACTNO"] = "EMP_CONTACTNO is required.";
            if (string.IsNullOrWhiteSpace(employee.EMP_CONTACTMAIL))
                validationErrors["EMP_CONTACTMAIL"] = "EMP_CONTACTMAIL is required.";
            if (string.IsNullOrWhiteSpace(employee.EMP_STATUS))
                validationErrors["EMP_STATUS"] = "EMP_STATUS is required.";
            if (string.IsNullOrWhiteSpace(employee.EMP_PASSWORD))
                validationErrors["EMP_PASSWORD"] = "EMP_PASSWORD is required.";
            if (string.IsNullOrWhiteSpace(employee.Reporting_Manager))
                validationErrors["Reporting_Manager"] = "Reporting_Manager is required.";
            if (string.IsNullOrWhiteSpace(employee.Role))
                validationErrors["Role"] = "Role is required.";

        }
        private bool IsValid => validationErrors.Count == 0;

        protected async void EditModel(int id)
        {

            var empl = await _context.Employee.FindAsync(id);
            if (empl == null)
            {
                Console.WriteLine("Customer not found");
                return;
            }
            if (empl != null)
            {
                EMP_ID = empl.EMP_ID;
                EMP_CODE = empl.EMP_CODE;
                EMP_NAME = empl.EMP_NAME;
                EMP_CONTACTNO = empl.EMP_CONTACTNO;
                EMP_CONTACTMAIL = empl.EMP_CONTACTMAIL;
                EMP_DESIGNATION = empl.EMP_DESIGNATION;
                Reporting_Manager = empl.Reporting_Manager;
                EMP_STATUS = empl.EMP_STATUS;
                Role = empl.Role;
                EMP_PASSWORD = empl.EMP_PASSWORD;
            }
            StateHasChanged();

        }

        async Task EditItem()
        {

            var existingemployee = _context.Employee.Where(e => e.EMP_ID == EMP_ID).FirstOrDefault();

            existingemployee.EMP_CODE = EMP_CODE;
            existingemployee.EMP_NAME = EMP_NAME;
            existingemployee.EMP_CONTACTNO = EMP_CONTACTNO;
            existingemployee.EMP_CONTACTMAIL = EMP_CONTACTMAIL;
            existingemployee.EMP_DESIGNATION = EMP_DESIGNATION;
            existingemployee.Reporting_Manager = Reporting_Manager;
            existingemployee.EMP_STATUS = EMP_STATUS;
            existingemployee.Role = Role;
            existingemployee.EMP_PASSWORD = EMP_PASSWORD;


            await employeeservice.UpdateEmployeeAsync(existingemployee);

            await OnInitializedAsync();
            StateHasChanged();
            await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Updated successfully");
        }

        protected async void btn_DeleteItem_Click(int id)
        {

            await employeeservice.RemoveEmployeeAsync(id);
            await OnInitializedAsync();
            StateHasChanged();
            await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Removed", "Removed successfully");

        }


        private void ClearAll()
        {
            EMP_CODE = "";
            EMP_NAME = string.Empty;
            EMP_CONTACTNO = string.Empty;
            EMP_CONTACTMAIL = string.Empty;
            EMP_DESIGNATION = string.Empty;
            EMP_STATUS = null;
        }

        private void ShowDeleteConfirmation(int itemId)
        {
            itemToDelete = itemId;
            StateHasChanged();
        }

        private async Task ConfirmDelete()
        {
            if (itemToDelete.HasValue)
            {
                btn_DeleteItem_Click(itemToDelete.Value);
                itemToDelete = null;
            }
        }
    }
}
