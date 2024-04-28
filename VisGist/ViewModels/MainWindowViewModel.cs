using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using Octokit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using VisGist.Enums;
using VisGist.Services;

namespace VisGist.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
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
        public ObservableCollection<Gist> Gists { get; set; } = new();

        #endregion End: PROPERTIES ---------------------------------------------------------------------------------


        #region EVENTS =========================================================================================

        internal delegate void VSThemeChanged(bool aDarkModeTheme);
        internal event VSThemeChanged vsThemeChanged;

        #endregion End: EVENTS ---------------------------------------------------------------------------------


        #region COMMANDS =========================================================================================

        public IAsyncRelayCommand GitAuthenticateCMD { get; set; }
        public IRelayCommand LogOutCMD {  get; set; }
        public IAsyncRelayCommand DoPostLoadActionsCMD {  get; set; }
        public IAsyncRelayCommand DoTestActionCMD {  get; set; }

        #endregion End: COMMANDS ---------------------------------------------------------------------------------


        #region OPERATIONAL PRIVATE VARS ===========================================================================
        private GistClientService gitClientService = new GistClientService();
        private General userVsOptions;


        #endregion End: OPERATIONAL PRIVATE VARS 


        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            SetupCommands();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateUserAsync);
            LogOutCMD = new RelayCommand(LogOut);
            DoPostLoadActionsCMD = new AsyncRelayCommand(OnViewLoadedAsync);
            DoTestActionCMD = new AsyncRelayCommand(DoTestActionAsync);
        }

        private async Task DoTestActionAsync()
        {
           await gitClientService.DoTestActionAsync(this);
        }

        private async Task OnViewLoadedAsync()
        {
            userVsOptions = await General.GetLiveInstanceAsync();
            if (userVsOptions.AutoLogin) await AuthenticateUserAsync();
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            IsDarkMode = Helpers.UI.IsDarkMode();
        }



        private async Task AuthenticateUserAsync()
        {
            var authenitcationResult = await gitClientService.AuthenticateAsync();

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
            gitClientService.Logout();
            StatusImage = StatusImage.Information;
            StatusText = $"Logged Out Successfully";
            IsAuthenticated = false;
        }


    }
}
