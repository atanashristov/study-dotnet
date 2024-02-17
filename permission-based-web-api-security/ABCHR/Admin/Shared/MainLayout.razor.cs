using MudBlazor;

namespace Admin.Shared
{
    public partial class MainLayout
    {
        public bool _drawerOpen = true;

        protected override void OnInitialized()
        {
            _interceptor.RegisterEvent();
            StateHasChanged();
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Components.Dialogs.Logout.ConfirmationMessage), "Logout Confirmation" },
                {nameof(Components.Dialogs.Logout.ButtonText), "Logout"},
                {nameof(Components.Dialogs.Logout.Color), Color.Error},
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Components.Dialogs.Logout>("Logout", parameters, options);
        }
    }
}
