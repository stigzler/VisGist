using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using System.ComponentModel;
using VisGist.Enums;
using VisGist.Services;

namespace VisGist.ViewModels
{
    internal class MainWindowViewModel : ObservableObject, INotifyPropertyChanged
    {

        #region PROPERTIES =========================================================================================

        // Private members
        private bool isDarkMode;
        private bool isAuthenticated = false;
        private StatusImage statusImage = StatusImage.Information;
        private string statusText = "Welcome to VisGist";

        // Public members
        public bool IsDarkMode { get => isDarkMode; set => SetProperty(ref isDarkMode, value); } 
        public bool IsAuthenticated {get => isAuthenticated; set => SetProperty(ref isAuthenticated, value); }
        public StatusImage StatusImage { get => statusImage; set => SetProperty(ref statusImage, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public string HelloWorld { get; set; } = "Hello World";

        #endregion End: PROPERTIES ---------------------------------------------------------------------------------


        #region EVENTS =========================================================================================

        internal delegate void VSThemeChanged(bool aDarkModeTheme);
        internal event VSThemeChanged vsThemeChanged;

        #endregion End: EVENTS ---------------------------------------------------------------------------------


        #region COMMANDS =========================================================================================

        public IAsyncRelayCommand GitAuthenticateCMD { get; set; }
        public IRelayCommand LogOutCMD {  get; set; }

        #endregion End: COMMANDS ---------------------------------------------------------------------------------


        private GistClientService GistClientService = new GistClientService();

        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            SetupCommands();
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            IsDarkMode = Helpers.UI.IsDarkMode();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateUserAsync);
            LogOutCMD = new RelayCommand(LogOut);
        }

        private async Task AuthenticateUserAsync()
        {
            var authenitcationResult = await GistClientService.AuthenticateAsync();

            if (authenitcationResult == null)
            {
                StatusImage = StatusImage.Success;
                StatusText = $"Logged In Successfully";
                IsAuthenticated = true;
            }
            else
            {
                StatusImage = StatusImage.Warning;
                StatusText = $"Authentication Error: {authenitcationResult.Message}";
                IsAuthenticated = false;
            }      
        }

        private void LogOut()
        {
            GistClientService.Logout();
            StatusImage = StatusImage.Information;
            StatusText = $"Logged Out Successfully";
            IsAuthenticated = false;
        }


    }
}
