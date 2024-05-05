using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using VisGist.Enums;
using VisGist.Services;

namespace VisGist.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {

        #region PROPERTIES =========================================================================================

        // Private backing members -----------------------------------------------------------------------------------------
        private bool isDarkMode;
        private bool isAuthenticated = false;
        private StatusImage statusImage = StatusImage.Information;
        private string statusText = "Welcome to VisGist";
        private bool statusBarVisible = false;

        private ObservableCollection<GistViewModel> gists = new ObservableCollection<GistViewModel>();

        private ViewModelBase selectedGistVmItem;
        private GistViewModel selectedGistViewModel;
        private GistFileViewModel selectedGistFileViewModel;

        private GridResizeDirection browserEditorsSplitterDirection = GridResizeDirection.Rows;
        private bool layoutHorizontal = false;
        private Font codeFont = new Font("Consolas", 14);

        // Public members
        public bool IsDarkMode { get => isDarkMode; set => SetProperty(ref isDarkMode, value); }
        public bool IsAuthenticated { get => isAuthenticated; set => SetProperty(ref isAuthenticated, value); }
        public StatusImage StatusImage { get => statusImage; set => SetProperty(ref statusImage, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public bool StatusBarVisible { get => statusBarVisible; set => SetProperty(ref statusBarVisible, value); }

        // below = ViewMOdelBase because selected item can be GistViewModel or GistFileViewModel (obtained from TreeView)
        public GistViewModel SelectedGistViewModel { get => selectedGistViewModel; set => SetProperty(ref selectedGistViewModel, value); }
        public GistFileViewModel SelectedGistFileViewModel
        {
            get => selectedGistFileViewModel;
            set => SetProperty(ref selectedGistFileViewModel, value);
        }
        public bool LayoutHorizontal { get => layoutHorizontal; set => SetProperty(ref layoutHorizontal, value); }

        public GridResizeDirection BrowserEditorsSplitterDirection
        {
            get => browserEditorsSplitterDirection;
            set => SetProperty(ref browserEditorsSplitterDirection, value);
        }
        public ObservableCollection<GistViewModel> Gists { get => gists; set => SetProperty(ref gists, value); }

        public ViewModelBase SelectedGistVmItem
        {
            get { return selectedGistVmItem; }
            set
            {
                SetProperty(ref selectedGistVmItem, value);
                OnSelectedGistItemChanged(value);
            }
        }

        public Font CodeFont
        {
            get
            {
                return codeFont;
            }
            private set
            {
                if (value == null) SetProperty(ref codeFont, new Font(FontFamily.GenericMonospace, 12));
                else SetProperty(ref codeFont, value);
            }
        }

        #endregion End: PROPERTIES ---------------------------------------------------------------------------------

        #region OPERATIONAL PRIVATE VARS ===========================================================================

        private GistClientService gitClientService = new GistClientService();
        private GistManager gistManager;
        private General userVsOptions = new General();

        #endregion End: OPERATIONAL PRIVATE VARS 


        #region EVENTS =========================================================================================

        internal delegate void VSThemeChanged(bool aDarkModeTheme);
        internal event VSThemeChanged vsThemeChanged;

        #endregion End: EVENTS ---------------------------------------------------------------------------------


        #region COMMANDS =========================================================================================

        public IAsyncRelayCommand GitAuthenticateCMD { get; set; }
        public IRelayCommand LogOutCMD { get; set; }
        public IAsyncRelayCommand DoPostLoadActionsCMD { get; set; }
        public IAsyncRelayCommand DoTestActionCMD { get; set; }
        public IAsyncRelayCommand GetAllGistsCMD { get; set; }
        public IAsyncRelayCommand AddNewGistCMD { get; set; }
        public IAsyncRelayCommand DeleteGistCMD { get; set; }

        #endregion End: COMMANDS ---------------------------------------------------------------------------------



        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;


            gistManager = new GistManager(gitClientService);

            SetupCommands();
        }

        private async Task OnViewLoadedAsync()
        {
            userVsOptions = await General.GetLiveInstanceAsync();
            CodeFont = userVsOptions.CodeFont;


            if (userVsOptions.AutoLogin) await AuthenticateUserAsync();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateUserAsync);
            LogOutCMD = new RelayCommand(LogOut);
            DoPostLoadActionsCMD = new AsyncRelayCommand(OnViewLoadedAsync);
            GetAllGistsCMD = new AsyncRelayCommand(GetAllGistsAsync);
            AddNewGistCMD = new AsyncRelayCommand(AddNewGistAsync);
            DeleteGistCMD = new AsyncRelayCommand(DeleteGistAsync);
            DoTestActionCMD = new AsyncRelayCommand(DoTestActionAsync);
        }

        private async Task DeleteGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Deleting Gist", true);

            await gistManager.DeleteGistAsync(SelectedGistViewModel);

            gists.Remove(SelectedGistViewModel);

            UpdateStatusBar(StatusImage.Success, "Gist deleted", false);
        }

        private async Task AddNewGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Adding New Gist", true);

            GistViewModel gistViewModel = await gistManager.CreateNewGistAsync(userVsOptions.NewGistPublic);

            //gists.Add(gistViewModel);

            gists.Insert(0, gistViewModel);

            UpdateStatusBar(StatusImage.Success, "New Gist added successfully", false);

        }

        private void OnSelectedGistItemChanged(ViewModelBase gistItem)
        {
            if (gistItem is GistViewModel)
            {
                SelectedGistViewModel = (GistViewModel)gistItem;
            }
            else if (gistItem is GistFileViewModel)
            {
                SelectedGistFileViewModel = (GistFileViewModel)gistItem;
                SelectedGistViewModel = SelectedGistFileViewModel.ParentGistViewModel;
            }
        }

        private async Task GetAllGistsAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, $"Loading Gists..", true);

            Gists = await gistManager.LoadGistsAsync();

            UpdateStatusBar(StatusImage.Success, $"Gists Loaded Successfully", false);
        }

        private async Task DoTestActionAsync()
        {
            gists[0].GistFiles[0].Filename = "Zzzzz - aappp!";

            // LayoutHorizontal = !LayoutHorizontal;

            //BrowserEditorsSplitterDirection = GridResizeDirection.Columns;

            //await gitClientService.DoTestActionAsync(this);
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
