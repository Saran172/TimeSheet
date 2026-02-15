using DevExpress.Blazor;
using Microsoft.JSInterop;
using System.Globalization;
using Task_1.Dto;
using Task_1.Entities;

namespace Task_1.Pages.CustomerScreen
{
    public partial class CustomerPage
    {
        const string ExportFileName = "ExportResult";
        bool EditItemsEnabled { get; set; }
        int FocusedRowVisibleIndex { get; set; }
        private bool isLoading = true;
        Customer[] Data { get; set; }
        IGrid Grid { get; set; }
        public int action { get; set; }
        Customer editingCust = null;
        // private List<Customer> lt_costumer = new List<Customer>();
        private List<Customer> lt_custumer = new List<Customer>();
        private Customer customerData = new Customer();
        Customer customer = new Customer();


        public int Cust_Id { get; set; }
        public string Cust_Code { get; set; }
        public string Cust_Name { get; set; }
        public string? Cust_ContactNo { get; set; }
        public string? Cust_Email { get; set; }

        private int? itemToDelete;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);
            isLoading = false;
            await LoadGridDataAsync();
            StateHasChanged();
        }
        async Task LoadGridDataAsync()
        {
            var data = await customerservice.GetCustomerEditableAsync();
            Data = data.ToArray();
        }
       

        protected async void AddModel()
        {

            action = 0;
            lt_custumer.Clear();
            customer = new Customer();
            StateHasChanged();
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

        async Task AddCustomer()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(customer.Cust_ContactNo))
                {
                    customer.Cust_ContactNo = "0";
                }
                if(string.IsNullOrWhiteSpace(customer.Cust_Email))
                {
                    customer.Cust_Email = "Nil";
                }
                await customerservice.InsertCustomerAsync(customer);
                await LoadGridDataAsync();
                StateHasChanged();
                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }

        protected async void EditModel(int id)
        {
            try
            {
                var cust = await _context.Customer.FindAsync(id);
                if (cust == null)
                {
                    Console.WriteLine("Customer not found");
                    return;
                }
                if (cust != null)
                {
                    Cust_Id = cust.Cust_Id;
                    Cust_Code = cust.Cust_Code;
                    Cust_Name = cust.Cust_Name;
                    Cust_ContactNo = cust.Cust_ContactNo;
                    Cust_Email = cust.Cust_Email;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }


        }

        async Task UpdateCustomer()
        {
            try
            {
                var existingcustomer = _context.Customer.Where(e => e.Cust_Id == Cust_Id).FirstOrDefault();

                if(existingcustomer == null)
                {
                    await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Customer not found");
                    return;
                }
                var updateCustomer = new Customer
                {
                    Cust_Id = Cust_Id,
                    Cust_Code = Cust_Code,
                    Cust_Name = Cust_Name,
                    Cust_ContactNo = Cust_ContactNo,
                    Cust_Email = Cust_Email
                };

                await customerservice.UpdateCustomerAsync(updateCustomer);
                await LoadGridDataAsync();
                StateHasChanged();
                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Updated successfully");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }


        }

        private async Task btn_DeleteItem_Click(int id)
        {
            try
            {
                await customerservice.RemoveCustomerAsync(id);
                await LoadGridDataAsync();
                StateHasChanged();
                await JS.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Removed", "Deleted successfully");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
        }


        private async Task ConfirmDelete()
        {
            try
            {
                if (itemToDelete.HasValue)
                {
                    btn_DeleteItem_Click(itemToDelete.Value);
                    itemToDelete = null; 
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }

        }

    }
}
