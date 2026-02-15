using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using Task_1.Dto;
using Task_1.Entities;

namespace Task_1.Pages.WorkScreen
{
    public partial class WorkPage
    {
        public string EmpName { get; set; }
        public int EmployeeID { get; set; }
        const string ExportFileName = "Works";
        bool EditItemsEnabled { get; set; }
        int FocusedRowVisibleIndex { get; set; }
        private bool isLoading = true;
        IEnumerable<Work> Data { get; set; }
        IGrid Grid { get; set; }
        public int action { get; set; }
        Work editingWor = null;
        private List<Customer> lt_custumer { get; set; } = new List<Customer>();

        private List<Work> lt_work = new List<Work>();
        List<Work> workinglist = new List<Work>();

        private Customer customerData = new Customer();



        private List<Employee> lt_employee { get; set; } = new List<Employee>();
        Work work = new Work();
        private List<Project> lt_project { get; set; } = new List<Project>();
        public int customerid { get; set; }
        public DateOnly CheckTime { get; set; }

        public DateTime endtime { get; set; }
        public DateTime starttime { get; set; }

        public int Work_Id { get; set; }
        public DateTime DateTimeValue { get; set; } = DateTime.Today;
        public DateTime DateTimeValue1 { get; set; } = DateTime.Today;
        public string Emp_Name { get; set; }
        public int Emp_Id { get; set; }
        public string Cust_Name { get; set; }
        public string Proj_Name { get; set; }
        public string? Ticket_Number { get; set; }
        public string Work_Descriptions { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Total_Hours { get; set; }
        public int Manager_Id { get; set; }
        public string? Manager_Approval { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime Created_DateTime { get; set; }
        public DateTime Updated_On { get; set; }
        public DateTime Updated_DateTime { get; set; }
        private List<Project> filteredProjects = new List<Project>();

        private TimeSpan TimeDiff { get; set; }
        private TimeOnly timeggg { get; set; }

        private bool isEmployee;
        private bool isEditable = false;  //Date edit enable function

        private bool isticket { get; set; } = false;
        private bool islearning { get; set; } = false;
        private string selectedOption { get; set; } = "N";
        private string nowork { get; set; } = "N";
        private int? ticketint { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {

                selectedOption = "N";
                var authState = await CustomAuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    var allowedNames = new List<string> { "user" };
                    isEmployee = allowedNames.Any(name => string.Equals(user.Identity.Name, name, StringComparison.OrdinalIgnoreCase));
                }
                isLoading = false;
                EditStateService.OnChange += HandleEditStateChanged;
                DateTimeValue = DateTime.Today;

                customerid = customerData.Cust_Id;
                var customerlist = await custserverice.GetCustomerEditableAsync();
                lt_custumer = customerlist.ToList();

                var projects = await projectservice.GetProjectEditableAsync();
                lt_project = projects.ToList();

                EmpName = await SessionStorage.GetItemAsync<string>("EmpName");
                EmployeeID = await SessionStorage.GetItemAsync<int>("EmployeeID");

                workinglist = await workservice.workToday(EmployeeID);
                await GetEmployeesbyid();
                await UpdateGrid(DateTimeValue, DateTimeValue1);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", ex.Message);
            }
            finally
            {
                isLoading = false;
            }


        }
        async Task LoadGridDataAsync()
        {
            var data = await workservice.GetWorkEditableAsync();
            Data = data.AsEnumerable();
        }

        //Date edit enable function
        private void ToggleEditability()
        {
            EditStateService.ToggleEditability();
        }
        public void Dispose()
        {
            EditStateService.OnChange -= HandleEditStateChanged;
        }
        private void HandleEditStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }


        string DisplayFormat { get; } = string.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator) ? "HH:mm" : "h:mm tt";

        private void OnCustomerChanged(ChangeEventArgs e)
        {
            Cust_Name = e.Value.ToString();

            var selectedCustomer = lt_custumer.Where(c => c.Cust_Name == Cust_Name).FirstOrDefault();

            if (selectedCustomer != null)
            {
                filteredProjects = lt_project.Where(p => p.Cust_Code == selectedCustomer.Cust_Code).ToList();
            }
            else
            {
                filteredProjects.Clear();
            }
            work.Cust_Name = e.Value.ToString();
            work.Proj_Name = string.Empty;
        }

        private void OnProjectChanged(ChangeEventArgs e)
        {
            Proj_Name = e.Value.ToString();
            var selectedCustomer = lt_project.Where(c => c.Project_Name == Proj_Name).FirstOrDefault();
            work.Proj_Name = e.Value.ToString();
        }

        private void HandleProject(string p)
        {
            var selectedCustomer = lt_custumer.Where(c => c.Cust_Name == Cust_Name).FirstOrDefault();

            if (selectedCustomer != null)
            {
                filteredProjects = lt_project.Where(p => p.Cust_Code == selectedCustomer.Cust_Code).ToList();
            }
            else
            {
                filteredProjects.Clear();
            }
        }

        protected async void AddModel()
        {
            action = 0;
            lt_work.Clear();
            work = new Work
            {
                Emp_Id = EmployeeID,
                Emp_Name = EmpName

            };
            selectedOption = "Project";
            filteredProjects.Clear();
            StateHasChanged();
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

        private void AddStartTime(ChangeEventArgs e)
        {
            if (TimeSpan.TryParse(e.Value?.ToString(), out TimeSpan startTime))
            {
                work.Start_Time = work.Work_Date.Date.Add(startTime);
                TimeDiff = startTime;
                timeggg = TimeOnly.FromDateTime(work.Start_Time);
                AddCalculateTotalHours();
            }
        }
        private void AddEndTime(ChangeEventArgs e)
        {
            if (TimeSpan.TryParse(e.Value?.ToString(), out TimeSpan endTime))
            {
                work.End_Time = work.Work_Date.Date.Add(endTime);
                AddCalculateTotalHours();
            }
        }
        private void AddCalculateTotalHours()
        {
            var duration = work.End_Time - work.Start_Time;
            if (duration < TimeSpan.Zero)
            {

                duration += TimeSpan.FromDays(1);

            }


            //var hours = duration.Hours;
            var min = duration.TotalMinutes;

            if (min > 0)
            {
                work.Total_Hours = $"{min}";
            }

        }
        private void UpdateStartTime(ChangeEventArgs e)
        {
            if (TimeSpan.TryParse(e.Value?.ToString(), out TimeSpan startTime))
            {
                Start_Time = new DateTime(
                    Start_Time.Year,
                    Start_Time.Month,
                    Start_Time.Day,
                    startTime.Hours,
                    startTime.Minutes,
                    startTime.Seconds
                );
                TimeDiff = startTime;
                timeggg = TimeOnly.FromDateTime(Start_Time);
                End_Time = Start_Time;
                UpdateCalculateTotalHours();
            }
        }
        private void UpdateEndTime(ChangeEventArgs e)
        {
            if (TimeSpan.TryParse(e.Value?.ToString(), out TimeSpan endTime))
            {
                End_Time = new DateTime(
                    End_Time.Year,
                    End_Time.Month,
                    End_Time.Day,
                    endTime.Hours,
                    endTime.Minutes,
                    endTime.Seconds
                );
                UpdateCalculateTotalHours();
            }
        }
        private void UpdateCalculateTotalHours()
        {

            var duration = End_Time - Start_Time;
            if (duration < TimeSpan.Zero)
            {

                duration += TimeSpan.FromDays(1);
            }

            var min = duration.TotalMinutes;

            if (min > 0)
            {
                Total_Hours = $"{min}";
            }
        }


        private void OnOptionChanged(string value)
        {
            selectedOption = value;

            switch (selectedOption)
            {
                case "Project":
                    isticket = false;
                    ticketint = 0;
                    break;

                case "Ticket":
                    isticket = true;
                    ticketint = 1;
                    break;

                case "Learning":
                    isticket = false;
                    ticketint = -1;
                    break;
            }
        }

        public async Task SaveWork()
        {
            try
            {


                if (IsValid)
                {
                    starttime = work.Start_Time;
                    endtime = work.End_Time;

                    bool isNull = await workservice.WorkDateNull(work.Work_Date, EmployeeID);


                    if (!isticket)
                    {
                        if (ticketint == -1)
                        {
                            work.Cust_Name = null;
                            work.Proj_Name = null;
                            work.IsTicket = -1;
                            work.Ticket_Number = null;
                            if (work.Total_Hours == null || work.Work_Descriptions == null)
                            {
                                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                                return;
                            }
                        }
                        else
                        {
                            ValidateForm();
                            work.IsTicket = 0;
                            work.Ticket_Number = null;
                            if (string.IsNullOrWhiteSpace(work.Cust_Name) || string.IsNullOrWhiteSpace(work.Proj_Name) || string.IsNullOrWhiteSpace(work.Total_Hours) || string.IsNullOrWhiteSpace(work.Work_Descriptions))
                            {
                                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                                return;
                            }
                        }

                    }
                    if (isticket)
                    {
                        work.IsTicket = 1;
                        work.Proj_Name = null;
                        if (string.IsNullOrWhiteSpace(work.Cust_Name) || string.IsNullOrWhiteSpace(work.Ticket_Number) || work.Total_Hours == null || work.Work_Descriptions == null)
                        {
                            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                            return;
                        }

                    }
                    if (string.IsNullOrWhiteSpace(selectedOption))
                    {
                        await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Select Project OR Ticket OR Learning!");
                        return;
                    }



                    if (isNull)
                    {
                        int count = 0;
                        foreach (var works in workinglist)
                        {
                            if (starttime.TimeOfDay == works.Start_Time.TimeOfDay || endtime.TimeOfDay == works.End_Time.TimeOfDay)
                            {
                                count = count + 1;
                            }
                            else if (starttime.TimeOfDay >= works.Start_Time.TimeOfDay && endtime.TimeOfDay <= works.End_Time.TimeOfDay)
                            {
                                count = count + 1;
                            }
                            else if (starttime.TimeOfDay > endtime.TimeOfDay)
                            {
                                count = count + 1;
                            }

                        }
                        if (count == 0)
                        {
                            await workservice.InsertWorkAsync(work);
                            workinglist = await workservice.workToday(EmployeeID);
                            await GetEmployeesbyid();
                            await UpdateGrid(DateTimeValue, DateTimeValue1);
                            await workservice.Createworkinapproval(work);
                            StateHasChanged();
                            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");
                            navigation.NavigateTo("/wor", true);
                        }
                        else
                        {
                            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Time Can't Be Overwrite and overlapped");
                        }

                    }
                    else
                    {
                        await workservice.InsertWorkAsync(work);
                        workinglist = await workservice.workToday(EmployeeID);
                        await GetEmployeesbyid();
                        await UpdateGrid(DateTimeValue, DateTimeValue1);
                        await workservice.Createworkinapproval(work);
                        StateHasChanged();
                        await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Submitted successfully");
                        navigation.NavigateTo("/wor", true);
                    }
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                }
            }
            catch (Exception ex)
            {

                await JSRuntime.InvokeVoidAsync($"sweetAlertInterop.showError", "Error", ex.Message + "error in add method");
            }




        }

        private Dictionary<string, string> validationErrors = new();

        private void ValidateForm()
        {
            validationErrors.Clear();

            if (string.IsNullOrWhiteSpace(work.Proj_Name))
                validationErrors["Proj_Name"] = "Project is required.";
        }
        private bool IsValid => validationErrors.Count == 0;

        protected async void EditModel(int id)
        {
            Work wor;
            using (var context = await DbFactory.CreateDbContextAsync())
            {
                wor = await context.Work.FindAsync(id);
            }
            if (wor == null)
            {
                Console.WriteLine("Work not found");
                return;
            }
            if (wor != null)
            {
                Work_Id = wor.Work_Id;
                DateTimeValue = wor.Work_Date;
                Emp_Id = wor.Emp_Id;
                Emp_Name = wor.Emp_Name;

                ticketint = wor.IsTicket;
                if (ticketint == 1)
                {
                    isticket = true;
                    selectedOption = "Ticket";
                }
                else if (ticketint == -1)
                {
                    isticket = false;
                    selectedOption = "Learning";
                }
                else
                {
                    isticket = false;
                    selectedOption = "Project";
                }
                Ticket_Number = wor.Ticket_Number;
                Cust_Name = wor.Cust_Name;
                Proj_Name = wor.Proj_Name;
                Work_Descriptions = wor.Work_Descriptions;
                Start_Time = wor.Start_Time;
                End_Time = wor.End_Time;
                Total_Hours = wor.Total_Hours;
                Manager_Id = wor.Manager_Id;
                Manager_Approval = wor.Manager_Approval;
                Created_On = wor.Created_On;
                Created_DateTime = wor.Created_DateTime;
                Updated_On = wor.Updated_On;
                Updated_DateTime = wor.Updated_DateTime;

                HandleProject(Cust_Name);
            }
            StateHasChanged();

        }

        public async Task UpdateWork()
        {
            try
            {
                if (!isticket)
                {
                    if (ticketint == -1)
                    {
                        Cust_Name = null;
                        Proj_Name = null;
                        ticketint = -1;
                        Ticket_Number = null;
                        if (Total_Hours == null || string.IsNullOrWhiteSpace(Work_Descriptions))
                        {
                            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                            return;
                        }
                    }
                    else
                    {
                        ValidateForm();

                        ticketint = 0;
                        Ticket_Number = null;
                        if (string.IsNullOrWhiteSpace(Cust_Name) || string.IsNullOrWhiteSpace(Proj_Name) || string.IsNullOrWhiteSpace(Total_Hours) || string.IsNullOrWhiteSpace(Work_Descriptions))
                        {
                            await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                            return;
                        }
                    }

                }
                if (isticket)
                {
                    ticketint = 1;
                    Proj_Name = null;
                    if (string.IsNullOrWhiteSpace(Cust_Name) || string.IsNullOrWhiteSpace(Ticket_Number) || Total_Hours == null || string.IsNullOrWhiteSpace(Work_Descriptions))
                    {
                        await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Enter the required field!");
                        return;
                    }
                }
                if (string.IsNullOrWhiteSpace(selectedOption))
                {
                    await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Select Project OR Ticket OR Learning!");
                    return;
                }

                Work existingwork;
                using (var context = await DbFactory.CreateDbContextAsync())
                {
                    existingwork = context.Work.Where(e => e.Work_Id == Work_Id).FirstOrDefault();
                }

                existingwork.Work_Id = Work_Id;
                existingwork.Work_Date = DateTimeValue;
                existingwork.Emp_Id = Emp_Id;
                existingwork.IsTicket = ticketint;
                if (ticketint == 1)
                {

                    existingwork.Cust_Name = Cust_Name;
                    existingwork.Proj_Name = null;
                    existingwork.Ticket_Number = Ticket_Number;
                }
                else if (ticketint == -1)
                {
                    existingwork.Cust_Name = null;
                    existingwork.Proj_Name = null;
                    existingwork.Ticket_Number = null;
                }
                else
                {

                    existingwork.Cust_Name = Cust_Name;
                    existingwork.Proj_Name = Proj_Name;
                    existingwork.Ticket_Number = null;
                }
                existingwork.Work_Descriptions = Work_Descriptions;
                existingwork.Start_Time = Start_Time;
                existingwork.End_Time = End_Time;
                existingwork.Total_Hours = Total_Hours;
                existingwork.Manager_Id = Manager_Id;
                existingwork.Manager_Approval = Manager_Approval;
                existingwork.Created_On = Created_On;
                existingwork.Created_DateTime = Created_DateTime;
                existingwork.Updated_On = Updated_On;
                existingwork.Updated_DateTime = Updated_DateTime;


                await workservice.UpdateWorkAsync(existingwork);

                // _context.Customer.Update(existingcustomer);
                //  _context.SaveChanges();
                await GetEmployeesbyid();
                await UpdateGrid(DateTimeValue, DateTimeValue1);
                await OnInitializedAsync();
                navigation.NavigateTo("/wor", true);
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Updated successfully");

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

        private async Task UpdateGrid(DateTime date1, DateTime date2)
        {
            int id = EmployeeID;
            var data = await workservice.GetWorkbybetweendates(date1, date2, id);
            //var data = await workservice.GetWorkbydateid(date, id);
            Data = data.AsEnumerable();

        }

        private async Task GetEmployeesbyid()
        {
            var emp = await workservice.GetWorkByEmpid(EmployeeID);
            Data = emp.ToArray();
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
