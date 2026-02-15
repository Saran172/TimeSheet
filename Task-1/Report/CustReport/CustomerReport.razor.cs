using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Task_1.Entities;

namespace Task_1.Report.CustReport
{
    public partial class CustomerReport
    {
        IGrid Grid;
        const string ExportFileName = "ExportResult";
        IEnumerable<Customer> DataSource { get; set; }
        IEnumerable<Customer> lt_customer { get; set; }
        private bool isLoading = true;
        // IEnumerable<> ProjectList { get; set; }
        
        bool PopupVisible = false;
        string PopupHeader;
        string PopupContent;
        // static List<IssueStatus?> StatusList { get; set; } =
        // ((IssueStatus[])Enum.GetValues(typeof(IssueStatus))).Cast<IssueStatus?>().ToList();
        string GridSearchText = "";
        [Parameter]
        public SizeMode SizeMode { get; set; }
        [Parameter]
        public EventCallback<Customer> GotoDetailsView { get; set; }
        void GotoDetailsViewClick(Customer issue)
        {
            PopupHeader = issue.Cust_Name;
            PopupContent = issue.Cust_Code;
            PopupVisible = true;
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = false;
                DataSource = await IssuesDataService.GetCustomerEditableAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
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
       
    }
}
