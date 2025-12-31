using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Admin.Extensions;
using Common.Authorization;
using Common.Responses.Identity;
using MudBlazor;
using Common.Responses.Employees;

namespace Admin.Pages
{
    public partial class EmployeeList
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private List<EmployeeResponse> _employeeList = new();
        private EmployeeResponse _employee = new();

        private bool _canCreateEmployees;
        private bool _canUpdateEmployees;
        private bool _canDeleteEmployees;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            _canCreateEmployees = await AuthService.HasPermissionAsync(user, AppFeature.Employees, AppAction.Read);
            _canUpdateEmployees = await AuthService.HasPermissionAsync(user, AppFeature.Employees, AppAction.Update);
            _canDeleteEmployees = await AuthService.HasPermissionAsync(user, AppFeature.Employees, AppAction.Delete);

            await GetEmployeesAsync();
            _loaded = true;
        }

        private async Task GetEmployeesAsync()
        {
            var response = await _employeeService.GetAllAsync();
            if (response.IsSuccessful)
            {
                _employeeList = response.ResponseData;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void Cancel()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}
