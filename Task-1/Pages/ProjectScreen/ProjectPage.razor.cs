using DevExpress.Blazor;
using Microsoft.JSInterop;
using System.Globalization;
using Task_1.Dto;
using Task_1.Entities;

namespace Task_1.Pages.ProjectScreen
{
    public partial class ProjectPage
    {
        const string ExportFileName = "ProjectExcel";
        bool EditItemsEnabled { get; set; }
        int FocusedRowVisibleIndex { get; set; }
        private bool isLoading = true;
        Project[] Data { get; set; }
        IGrid Grid { get; set; }
        public int action { get; set; }
        Project editingCust = null;
        private List<Project> lt_project = new List<Project>();
        private CustomerDto customerData = new CustomerDto();
        Project project = new Project();
        private List<Customer> lt_customer { get; set; } = new List<Customer>();

        public int Proj_Id { get; set; }
        public int So_Number { get; set; }
        public DateTime So_Date { get; set; }
        public string Cust_Code { get; set; }
        public string Project_Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Project_Status { get; set; }
        public string? isbillable { get; set; }

        private int? itemToDelete;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = false;
                lt_customer = (await customerservice.GetCustomerEditableAsync()).ToList();
                await LoadGridDataAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            StateHasChanged();
            
        }
        async Task LoadGridDataAsync()
        {
            var data = await projectservice.GetProjectEditableAsync();
            var customerLookup = lt_customer.ToDictionary(c => c.Cust_Code, c => c.Cust_Name);

            var mappedData = data.Select(proj => new Project
            {
                Proj_Id = proj.Proj_Id,
                So_Date = proj.So_Date,
                So_Number = proj.So_Number,
                Cust_Code = proj.Cust_Code,
                Cust_Name = customerLookup.ContainsKey(proj.Cust_Code)
                               ? customerLookup[proj.Cust_Code]
                               : string.Empty,
                Project_Name = proj.Project_Name,
                Project_Status = proj.Project_Status,
                DueDate = proj.DueDate,
                IsBillable = proj.IsBillable
            });

            Data = mappedData.ToArray();
        }
        TimeSpan TimeValue { get; set; } = DateTime.Now.TimeOfDay;
        string DisplayFormat { get; } = string.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator) ? "HH:mm" : "h:mm tt";

        protected async void AddModel()
        {

            action = 0;
            lt_project.Clear();
            project = new Project();
            StateHasChanged();
        }
        // protected async void EditModel()
        // {

        //     action = 1;
        //     customer = new Customer();
        //     await LoadGridDataAsync();
        //     StateHasChanged();
        // }
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
            // await customerservice.RemoveCustomerAsync((Customer)e.DataItem);
            await LoadGridDataAsync();
            if (Data.Length == 0)
                UpdateEditItemsEnabled(false);
        }
        async Task Grid_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                await projectservice.InsertProjectAsync((Project)e.EditModel);
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

        async Task AddPItem()
        {
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(project.Project_Name))
                    errors.Add("Project Name is required.");
                if (string.IsNullOrWhiteSpace(project.Cust_Code))
                    errors.Add("Customer Code is required.");
                if (string.IsNullOrWhiteSpace(project.Project_Status))
                    errors.Add("Project Status is required.");
                if (project.DueDate == default)
                    errors.Add("Due Date is required.");
                if (project.So_Number <= 0)
                    errors.Add("SO Number is required and must be greater than zero.");
                if (project.So_Date == default)
                    errors.Add("SO Date is required.");
                if (string.IsNullOrWhiteSpace(project.IsBillable))
                    errors.Add("IsBillable must be specified.");

                if (errors.Count > 0)
                {
                    await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Please fill required fields!");
                    return;
                }

                await projectservice.InsertProjectAsync(project);
                await LoadGridDataAsync();
                StateHasChanged();
                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
           
        }

        protected async void EditPModel(int id)
        {

            var proj = await _context.Project.FindAsync(id);

            if (proj == null)
            {
                Console.WriteLine("Project not found");
                return;
            }
            if (proj != null)
            {
                Proj_Id = proj.Proj_Id;
                So_Number = proj.So_Number;
                So_Date = proj.So_Date;
                Cust_Code = proj.Cust_Code;
                Project_Name = proj.Project_Name;
                DueDate = proj.DueDate;
                Project_Status = proj.Project_Status;
                isbillable = proj.IsBillable;
            }

            StateHasChanged();
        }

        async Task EditPItem()
        {

            var existingproject = _context.Project.Where(e => e.Proj_Id == Proj_Id).FirstOrDefault();

            existingproject.Proj_Id = Proj_Id;
            existingproject.So_Number = So_Number;
            existingproject.So_Date = So_Date;
            existingproject.Cust_Code = Cust_Code;
            existingproject.Project_Name = Project_Name;
            existingproject.DueDate = DueDate;
            existingproject.Project_Status = Project_Status;
            existingproject.IsBillable = isbillable;


            await projectservice.UpdateProjectAsync(existingproject);

            await LoadGridDataAsync();
            StateHasChanged();
            await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Updated successfully");
        }

        protected async void btn_DeletePItem_Click(int id)
        {

            await projectservice.RemoveProjectAsync(id);
            await LoadGridDataAsync();
            StateHasChanged();
            await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Deleted successfully");

        }



        private void ShowDeleteConfirmation(int itemId)
        {
            itemToDelete = itemId;
            // Show the modal
            StateHasChanged(); // Refresh UI to show the modal
        }

        private async Task ConfirmDelete()
        {
            if (itemToDelete.HasValue)
            {
                btn_DeletePItem_Click(itemToDelete.Value);
                itemToDelete = null; // Clear the item
            }
            StateHasChanged();
        }
    }
}
