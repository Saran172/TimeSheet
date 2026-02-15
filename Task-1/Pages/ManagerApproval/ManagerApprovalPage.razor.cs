using DevExpress.Blazor;
using Microsoft.JSInterop;
using System.Data;
using System.Data.SqlClient;
using Task_1.Entities;

namespace Task_1.Pages.ManagerApproval
{
    public partial class ManagerApprovalPage
    {


        public string ManagerName { get; set; }
        private int EmployeeID { get; set; }
        const string ExportFileName = "ExportResult";
        bool EditItemsEnabled { get; set; }
        int FocusedRowVisibleIndex { get; set; }
        private bool isLoading = true;
        Work[] Data { get; set; }
        IGrid Grid { get; set; }
        public int action { get; set; }
        Work editingWor = null;
        private List<Customer> lt_custumer { get; set; } = new List<Customer>();
        private List<Work> lt_work = new List<Work>();
        private Customer customerData = new Customer();

        Employee employee = new Employee();
        private List<Employee> lt_employee { get; set; } = new List<Employee>();
        private List<Employee> lt_employee1 { get; set; } = new List<Employee>();

        Work work = new Work();
        private List<Project> lt_project { get; set; } = new List<Project>();
        public int customerid { get; set; }
        public int Managerid { get; set; }

        public int Work_Id { get; set; }
        public DateTime DateStartTimeValue { get; set; }
        public DateTime DateEndTimeValue { get; set; }
        public string Emp_Name { get; set; }
        public int Emp_Id { get; set; }
        public string Cust_Name { get; set; }
        public string Proj_Name { get; set; }
        public string Work_Descriptions { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Total_Hours { get; set; }
        public int Manager_Id { get; set; }
        public string Manager_Name { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public string? Manager_Approval { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime Created_DateTime { get; set; }
        public DateTime Updated_On { get; set; }
        public DateTime Updated_DateTime { get; set; }

        private int pendingWorkCount;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = false;
                await LoadPendingWorkCountAsync();
                DateStartTimeValue = DateTime.Today.AddDays(-1);
                DateEndTimeValue = DateTime.Today;
                // await LoadGridDataAsync();
                lt_custumer = (await custserverice.GetCustomerEditableAsync()).ToList();
                lt_project = (await projectservice.GetProjectEditableAsync()).ToList();
                lt_employee = (await getdata()).ToList();
                lt_employee1 = (await getmang()).ToList();
                EmployeeID = await SessionStorage.GetItemAsync<int>("EmployeeID");
                ManagerName = await SessionStorage.GetItemAsync<string>("ManagerName");
                await LoadGridDataAsync();
                //await UpdateGrid(DateStartTimeValue,DateEndTimeValue);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }

        async Task LoadGridDataAsync()
        {
            if (ManagerName == "Ramesh")
            {
                var data = await workservice.GetWorkbydesignationandApproval();
                Data = data.ToArray();


            }
            else if (ManagerName == "Gopinath")
            {
                var data = await workservice.GetWorkEditableAsync();
                Data = data.ToArray();

            }
            else if (ManagerName == "Sindhu")
            {
                var data = await workservice.GetWorkbytestdesignationandApproval();
                Data = data.ToArray();
            }
            else if (ManagerName == "user")
            {
                var data = await workservice.GetWorkEditableAsync();
                Data = data.ToArray();

            }
            else
            {
                var data = await workservice.GetWorkEditableAsync();
                Data = data.ToArray();
            }

        }

        private async Task LoadPendingWorkCountAsync()
        {
            pendingWorkCount = await workservice.GetPendingWorkCountAsync();
        }

        protected async void AddModel()
        {

            action = 0;
            lt_work.Clear();
            work = new Work();
            StateHasChanged();
        }

        protected async void PrepareAction(int id, int actionType)
        {
            try
            {
                Work_Id = 0;
                action = 0;
                Work_Descriptions = string.Empty;
                Correction = string.Empty;

                var wor = await _context.Work.FindAsync(id);

                Work_Id = id;
                action = actionType;
                Work_Descriptions = wor.Work_Descriptions;
                Correction = wor.Correction;

                if (action == 1)
                {
                    SubmitAction();
                }
                else
                {
                    JSRuntime.InvokeVoidAsync("eval", "$('#exampleModal').modal('show')");
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }
        private async Task SubmitAction()
        {
            // Perform the action based on the current action state
            switch (action)
            {
                case 1:
                    await Accept(Work_Id);
                    break;
                case 2:
                    await Hold(Work_Id);
                    break;
                case 3:
                    await Reject(Work_Id);
                    break;
            }
            try
            {
                action = 0;
                Comment = string.Empty;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            StateHasChanged();

        }


        protected async Task Accept(int id)
        {
            try
            {
                var wor = await _context.Work.FindAsync(id);
                wor.Manager_Id = EmployeeID;
                wor.Manager_Name = ManagerName;
                wor.Manager_Approval = "Approved";
                wor.Comment = Comment;

                await workservice.UpdateWorkAsync(wor);
                await UpdateGrid(DateStartTimeValue, DateEndTimeValue);
                await LoadGridDataAsync();
                workservice.Updateworkinapproval(wor);
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Approved successfully");
                Grid.Reload();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }
        protected async Task Reject(int id)
        {
            try
            {
                var wor = await _context.Work.FindAsync(id);
                wor.Manager_Id = EmployeeID;
                wor.Manager_Name = ManagerName;
                wor.Manager_Approval = "Reject";
                wor.Comment = Comment;

                await workservice.UpdateWorkAsync(wor);
                await UpdateGrid(DateStartTimeValue, DateEndTimeValue);
                await LoadGridDataAsync();
                workservice.Updateworkinapproval(wor);
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Reject", "Rejected successfully");
                Grid.Reload();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }
        protected async Task Hold(int id)
        {
            try
            {
                var wor = await _context.Work.FindAsync(id);
                wor.Manager_Id = EmployeeID;
                wor.Manager_Name = ManagerName;
                wor.Manager_Approval = "Hold";
                wor.Comment = Comment;

                await workservice.UpdateWorkAsync(wor);
                await UpdateGrid(DateStartTimeValue, DateEndTimeValue);
                await LoadGridDataAsync();
                workservice.Updateworkinapproval(wor);
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Hold", "Work has been put on hold");
                Grid.Reload();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }

        private void ShowDetails(int workId)
        {
            try
            {
                var workItem = Data.FirstOrDefault(w => w.Work_Id == workId);
                if (workItem != null)
                {
                    Emp_Name = workItem.Emp_Name;
                    Work_Descriptions = workItem.Work_Descriptions;
                    Cust_Name = workItem.Cust_Name;
                    Proj_Name = workItem.Proj_Name;
                    Start_Time = workItem.Start_Time;
                    End_Time = workItem.End_Time;
                    Total_Hours = workItem.Total_Hours;
                }
            }
            catch (Exception ex)
            {
                JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
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
                var newEmployee = (Customer)e.EditModel;
                newEmployee.Cust_Name = "";

            }
        }
        async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            await LoadGridDataAsync();
            if (Data.Length == 0)
                UpdateEditItemsEnabled(false);
        }
        async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                await workservice.InsertWorkAsync((Work)e.EditModel);
                UpdateEditItemsEnabled(true);
            }
            else
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
            await workservice.InsertWorkAsync(work);
            await LoadGridDataAsync();
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");
        }

        protected async void EditModel(int id)
        {

            var wor = await _context.Work.FindAsync(id);
            if (wor == null)
            {
                Console.WriteLine("Work not found");
                return;
            }
            if (wor != null)
            {
                Work_Id = wor.Work_Id;
                DateStartTimeValue = wor.Work_Date;
                Emp_Id = wor.Emp_Id;
                Cust_Name = wor.Cust_Name;
                Proj_Name = wor.Proj_Name;
                Work_Descriptions = wor.Work_Descriptions;
                Start_Time = wor.Start_Time;
                End_Time = wor.End_Time;
                Total_Hours = wor.Total_Hours;
                Manager_Id = EmployeeID;
                Manager_Name = ManagerName;
                Manager_Approval = wor.Manager_Approval;
                Comment = wor.Comment;
                Created_On = wor.Created_On;
                Created_DateTime = wor.Created_DateTime;
                Updated_On = wor.Updated_On;
                Updated_DateTime = wor.Updated_DateTime;
            }
            StateHasChanged();

        }

        async Task EditItem()
        {


            work.Comment = Comment;

            try
            {
                await workservice.UpdateWorkAsync(work);
                await LoadGridDataAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }

        protected async void btn_DeleteItem_Click(int id)
        {

            await workservice.RemoveWorkAsync(id);
            await LoadGridDataAsync();
            StateHasChanged();

        }
        private async Task<IEnumerable<Employee>> getdata()

        {
            List<Employee> employees1 = new List<Employee>();
            IEnumerable<Employee> employees = employees1;
            try
            {

                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT EMP_ID FROM Employee where Role='manager'";

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
            return employees;
        }
        private async Task<IEnumerable<Employee>> getmang()

        {
            List<Employee> employees1 = new List<Employee>();
            IEnumerable<Employee> employees = employees1;
            try
            {

                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT EMP_NAME FROM Employee where Role='manager'";

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
            return employees;
        }
        private async Task UpdateGrid(DateTime date1, DateTime date2)
        {
            try
            {
                string empdesig = string.Empty;
                if (ManagerName == "Ramesh")
                {
                    var datas = await workservice.GetWorkbydateAndDesignation(date1, date2);
                    Data = datas.ToArray();
                }
                else if (ManagerName == "Sindhu")
                {
                    var datas = await workservice.GetWorkbydateAndDesignation1(date1, date2);
                    Data = datas.ToArray();
                }
                else
                {
                    var data = await workservice.GetWorkbydate(date1, date2);
                    Data = data.ToArray();
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            StateHasChanged();
        }

        private async Task LoadEmployeeWorkByDesig()
        {
            try
            {
                string connectionstring = Configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string query = "Select ";
                    SqlCommand command = new SqlCommand(query, connection);
                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // Data = await _data.ConvertToList<Work>(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}
