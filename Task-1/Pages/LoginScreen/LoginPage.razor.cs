using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Task_1.Authentication;
using Task_1.Entities;

namespace Task_1.Pages.LoginScreen
{
    public partial class LoginPage
    {
        private string PassInputType { get; set; }

        private bool IsLoading = false;
        private bool isForgotPasswordModalOpen = false;
        private string forgotPasswordUserName = string.Empty;
        private string newPassword = string.Empty;
        private string confirmPassword = string.Empty;
        private string contactNumber = string.Empty;
        private void OpenForgotPasswordModal() => isForgotPasswordModalOpen = true;

        private class Models
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private Models models = new Models();
        private Employee employee = new Employee();


        private async Task Authenticate()
        {
            try
            {
                IsLoading = true;

                var userAccount = await userAccountService.GetByUserNameAsync(employee.EMP_NAME);

                if (userAccount == null || !userAccountService.VerifyPassword(userAccount, employee.EMP_PASSWORD))
                {
                    await js.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Invalid Username or Password");
                    return;
                }

                if (userAccount.EMP_STATUS != "Active")
                {
                    await js.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "You are currently an inactive user");
                    return;
                }

                var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
                var emp = new Employee
                {
                    EMP_NAME = userAccount.EMP_NAME,
                    Role = userAccount.Role
                };
                await customAuthStateProvider.UpdateAuthenticationState(emp);

                await SessionStorage.SetItemAsync("EmployeeID", userAccount.EMP_ID);
                await SessionStorage.SetItemAsync("ManagerName", userAccount.EMP_NAME);
                await SessionStorage.SetItemAsync("EmpName", userAccount.EMP_NAME);

                navManager.NavigateTo("/", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login failed: " + ex.Message);
                await js.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "A system error occurred.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CloseForgotPasswordModal()
        {
            isForgotPasswordModalOpen = false;
            forgotPasswordUserName = string.Empty;
            newPassword = string.Empty;
            confirmPassword = string.Empty;
        }

        private async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(forgotPasswordUserName) ||
                string.IsNullOrWhiteSpace(contactNumber) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                newPassword != confirmPassword)
            {
                await js.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Invalid input or passwords do not match.");
                return;
            }

            var userAccount = await userAccountService.GetByUserNameAsync(forgotPasswordUserName);

            if (userAccount == null || userAccount.EMP_CONTACTNO != contactNumber)
            {
                await js.InvokeVoidAsync("sweetAlertInterop.showError", "Error", "Invalid user or contact number");
                return;
            }

            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
            userAccount.EMP_PASSWORD = passwordHasher.HashPassword(null!, newPassword);

            await employeeeservice.UpdateEmployeeAsyncc(userAccount);

            await js.InvokeVoidAsync("sweetAlertInterop.showSuccess", "Success", "Password has been reset successfully.");

            CloseForgotPasswordModal();
            await js.InvokeVoidAsync("checkpass");

            StateHasChanged();
        }
    }
}
