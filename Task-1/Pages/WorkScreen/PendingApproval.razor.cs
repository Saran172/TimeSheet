using DevExpress.Blazor;
using Microsoft.JSInterop;
using Task_1.Entities;

namespace Task_1.Pages.WorkScreen
{
    public partial class PendingApproval
    {
        private bool isLoading = true;
        IGrid Grid { get; set; }
        WorkApproval[] Data { get; set; }
        public string ManagerName { get; set; }
        public string? Comment { get; set; }
        public string? Correction { get; set; }
        public int Work_Id { get; set; }
        public int action { get; set; }
        private int EmployeeID { get; set; }
        public string EmpName { get; set; }

        private int holdWorkCount;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = false;
                ManagerName = await SessionStorage.GetItemAsync<string>("ManagerName");
                EmpName = await SessionStorage.GetItemAsync<string>("EmpName");
                EmployeeID = await SessionStorage.GetItemAsync<int>("EmployeeID");

                // var pendingApprovals = await LoadPendingApprovalsForEmployeeAsync();
                // Data = pendingApprovals.ToArray();
                await LoadGridDataAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            
        }

        async Task LoadGridDataAsync()
        {
            try
            {
                var data = await approvalservice.GetHoldWorkAsync(EmployeeID);
                Data = data.ToArray();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }


        }

        private void PrepareAction(int id, int actionType)
        {
            Work_Id = id;
            action = actionType;
            Comment = string.Empty;
            JSRuntime.InvokeVoidAsync("eval", "$('#exampleModal').modal('show')");
        }
        private async Task SubmitAction()
        {

            switch (action)
            {
                case 1:
                    await Resubmit(Work_Id);
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

        }
        protected async Task Resubmit(int id)
        {
            var wor = await _context.Work.FindAsync(id);
            wor.Correction = Correction;
            await workservice.UpdateWorkAsync(wor);
            workservice.Updateworkinapproval(wor);
            await LoadGridDataAsync();
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");

        }

        private async Task<List<WorkApproval>> LoadPendingApprovalsForEmployeeAsync()
        {
            return await approvalservice.GetPendingApprovalForEmployeeAsync(EmployeeID);
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

    }
}
