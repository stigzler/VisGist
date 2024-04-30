﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using Octokit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private bool statusBarVisible = false;
        private ObservableCollection<Models.Gist> gists = new ObservableCollection<Models.Gist>();

        // Public members
        public bool IsDarkMode { get => isDarkMode; set => SetProperty(ref isDarkMode, value); } 
        public bool IsAuthenticated {get => isAuthenticated; set => SetProperty(ref isAuthenticated, value); }
        public StatusImage StatusImage { get => statusImage; set => SetProperty(ref statusImage, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public bool StatusBarVisible { get => statusBarVisible; set => SetProperty(ref statusBarVisible, value); }  
        public string HelloWorld { get; set; } = "Hello World";
        public ObservableCollection<Models.Gist> Gists { get => gists; set => SetProperty(ref gists, value); }

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
        public IAsyncRelayCommand GetAllGistsCMD {  get; set; }

        #endregion End: COMMANDS ---------------------------------------------------------------------------------


        #region OPERATIONAL PRIVATE VARS ===========================================================================

        private GistClientService gitClientService = new GistClientService();
        private GistManager gistManager;
        private General userVsOptions;

        #endregion End: OPERATIONAL PRIVATE VARS 

        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            gistManager = new GistManager(gitClientService);

            SetupCommands();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateUserAsync);
            LogOutCMD = new RelayCommand(LogOut);
            DoPostLoadActionsCMD = new AsyncRelayCommand(OnViewLoadedAsync);    
            GetAllGistsCMD = new AsyncRelayCommand(GetAllGistsAsync);

            DoTestActionCMD = new AsyncRelayCommand(DoTestActionAsync);
        }

        private async Task GetAllGistsAsync()
        {
            UpdateStatusBar(StatusImage.Loading, $"Loading Gists..", true);

            Gists = await gistManager.LoadGistsAsync();

            UpdateStatusBar(StatusImage.Success, $"Gists Loaded Successfully", false);
        }

        private async Task DoTestActionAsync()
        {
            gists[0].GistFiles[0].Filename = "Zzzzz - aappp!";

            SetProperty(ref gists[0].GistFiles[0].Filename, )
           //await gitClientService.DoTestActionAsync(this);
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
                UpdateStatusBar(StatusImage.Success, $"Logged In Successfully");
                IsAuthenticated = true;
            }
            else
            {
                UpdateStatusBar(StatusImage.Warning, $"Authentication Error: {authenitcationResult.Message}");
                IsAuthenticated = false;
            }      
        }

        private void UpdateStatusBar(StatusImage stausImage, string statusText, bool progressBarVisible = false)
        {
            StatusImage = stausImage;
            StatusText = statusText;
            StatusBarVisible = progressBarVisible;
        }

        private void LogOut()
        {
            gitClientService.Logout();
            UpdateStatusBar(StatusImage.Information, $"Logged Out Successfully");
            IsAuthenticated = false;
        }


    }
}
