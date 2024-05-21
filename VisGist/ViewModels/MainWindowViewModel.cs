using CommunityToolkit.Mvvm.Input;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.RpcContracts.FileSystem;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using VisGist.Enums;
using VisGist.Services;
using Syncfusion.Windows.Edit;
using Languages = Syncfusion.Windows.Edit.Languages;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using Octokit;
using System.Media;

namespace VisGist.ViewModels
{
    internal partial class MainWindowViewModel : ViewModelBase
    {

        #region PROPERTIES =========================================================================================

        // Private backing members -----------------------------------------------------------------------------------------
        private bool isDarkMode;
        private bool isAuthenticated = false;
        private StatusImage statusImage = StatusImage.Information;
        private string statusText = "Welcome to VisGist";
        private bool statusBarVisible = false;
        private bool syntaxHighlightingEnabled = false;
        private Languages selectedLanguage = Syncfusion.Windows.Edit.Languages.Text;
        private bool codeNumberingVisible = false;
        private bool codeOutliningVisible = false;
        private bool codeFocused = false;

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

        public bool CodeFocused { get => codeFocused; set => SetProperty(ref codeFocused, value); }
        public bool CodeNumberingVisible { get => codeNumberingVisible; set => SetProperty(ref codeNumberingVisible, value); }
        public bool CodeOutliningVisible { get => codeOutliningVisible; set => SetProperty(ref codeOutliningVisible, value); }
        public StatusImage StatusImage { get => statusImage; set => SetProperty(ref statusImage, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public bool StatusBarVisible { get => statusBarVisible; set => SetProperty(ref statusBarVisible, value); }
        public bool SyntaxHighlightingEnabled { get => syntaxHighlightingEnabled; set => SetProperty(ref syntaxHighlightingEnabled, value); }
        public bool LayoutHorizontal { get => layoutHorizontal; set => SetProperty(ref layoutHorizontal, value); }
        public IEnumerable<Languages> Languages { get => Enum.GetValues(typeof(Languages)).Cast<Languages>(); }
        public GistViewModel SelectedGistViewModel { get => selectedGistViewModel; set => SetProperty(ref selectedGistViewModel, value); }
        public GistFileViewModel SelectedGistFileViewModel { get => selectedGistFileViewModel; set => SetProperty(ref selectedGistFileViewModel, value); }
        public Languages SelectedLanguage { get => selectedLanguage; set => SetProperty(ref selectedLanguage, value); }
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
                if (value == null) SetProperty(ref codeFont, new Font(System.Drawing.FontFamily.GenericMonospace, 12));
                else SetProperty(ref codeFont, value);
            }
        }

        // Commands ----------------------------------------------------------------------------------------------

        //public IRelayCommand SetCodeNumberingVisibleCMD { get => setCodeNumberingVisibleCMD; }

        #endregion End: PROPERTIES ---------------------------------------------------------------------------------

        #region OPERATIONAL PRIVATE VARS ===========================================================================

        private GistClientService gitClientService = new GistClientService();
        private GistManager gistManager;
        private General userVsOptions = new General();
        private CodeEditorManager codeEditorManager;

        #endregion End: OPERATIONAL PRIVATE VARS 

        #region EVENTS =========================================================================================

        internal delegate void VSThemeChanged(bool aDarkModeTheme);
        internal event VSThemeChanged vsThemeChanged;

        internal delegate void FilenameChange(bool duplicate);
        internal event FilenameChange FilenameChangeDetected;

        #endregion End: EVENTS ---------------------------------------------------------------------------------

        #region COMMANDS =========================================================================================

        public IAsyncRelayCommand GitAuthenticateCMD { get; set; }
        public IRelayCommand LogOutCMD { get; set; }
        public IAsyncRelayCommand DoPostLoadActionsCMD { get; set; }
        public IAsyncRelayCommand DoTestActionCMD { get; set; }
        public IAsyncRelayCommand GetAllGistsCMD { get; set; }
        public IAsyncRelayCommand AddNewGistCMD { get; set; }
        public IAsyncRelayCommand DeleteGistCMD { get; set; }
        public IAsyncRelayCommand DeleteGistFileCMD { get; set; }
        public IAsyncRelayCommand AddNewGistFileCMD { get; set; }
        public IAsyncRelayCommand SaveGistCMD { get; set; }
        public IRelayCommand SetSyntaxHighlightingCMD { get; set; }
        public IRelayCommand SetCodeNumberingVisibleCMD { get; set; }

        #endregion End: COMMANDS ---------------------------------------------------------------------------------


        // ==============================================================================================
        // CONSTRUCTOR
        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            // Setup Managers
            gistManager = new GistManager(gitClientService);
            //codeEditorManager = new CodeEditorManager()

            SetupCommands();
        }

        private async Task OnViewLoadedAsync()
        {
            userVsOptions = await General.GetLiveInstanceAsync();
            CodeFont = userVsOptions.CodeFont;

            if (userVsOptions.AutoLogin) await AuthenticateUserAsync();

            if (userVsOptions.AutoLoadGists) await GetAllGistsAsync();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateUserAsync);
            LogOutCMD = new RelayCommand(LogOut);
            DoPostLoadActionsCMD = new AsyncRelayCommand(OnViewLoadedAsync);
            GetAllGistsCMD = new AsyncRelayCommand(GetAllGistsAsync);
            AddNewGistCMD = new AsyncRelayCommand(AddNewGistAsync);
            AddNewGistFileCMD = new AsyncRelayCommand(AddNewGistFileAsync);
            DeleteGistCMD = new AsyncRelayCommand(DeleteGistAsync);
            DeleteGistFileCMD = new AsyncRelayCommand(DeleteGistFileAsync);
            DoTestActionCMD = new AsyncRelayCommand(DoTestActionAsync);
            SaveGistCMD = new AsyncRelayCommand(SaveGistAsync);
            SetSyntaxHighlightingCMD = new RelayCommand<bool>(SetSyntaxHighlighting);
            SetCodeNumberingVisibleCMD = new RelayCommand(SetCodeNumberingVisible);
        }

        private async Task DeleteGistFileAsync()
        {
            if (SelectedGistViewModel.GistFiles.Count == 1)
            {
                SystemSounds.Exclamation.Play();
                UpdateStatusBar(StatusImage.Warning, "Cannot delete last file. Delete Gist instead.", false);
                return;
            }

            SelectedGistFileViewModel.MarkedForDeletion = true;
            await SaveGistAsync();
            SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];
        }

        private async Task AddNewGistFileAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Adding New GistFile", true);
            GistFileViewModel gistFileViewModel = await gistManager.CreateNewGistFileAsync(SelectedGistViewModel);
            SelectedGistViewModel.AddGistFile(gistFileViewModel);
            await SaveGistAsync();
            SelectedGistViewModel.SortGistFiles();
            UpdateStatusBar(StatusImage.Success, "New GistFile added successfully", false);
        }

        private async Task AddNewGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Adding New Gist", true);

            GistViewModel gistViewModel = await gistManager.CreateNewGistAsync(userVsOptions.NewGistPublic);

            gists.Insert(0, gistViewModel);

            UpdateStatusBar(StatusImage.Success, "New Gist added successfully", false);
        }

        private void SetCodeNumberingVisible()
        {
            CodeNumberingVisible = !CodeNumberingVisible;
        }

        private void SetSyntaxHighlighting(bool obj)
        {
            SyntaxHighlightingEnabled = obj;
        }

        private async Task SaveGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Saving Gist", true);

            // store the name of the selected gist file as update essentially creates new gistfile object
            // breaking the link between the UI gistfile and the dataobject one
            string gistFilename = SelectedGistFileViewModel?.Filename;

            // Save (update) gist in git server
            Gist updatedGist = await gistManager.SaveGistAsync(SelectedGistViewModel);

            // Now update the GistViewModel (populates any properties figured server side like filesize etc)
            SelectedGistViewModel.UpdateGistFile(updatedGist);

            // Now update starred status of GistViewModel (as this isn't stored in the Gist data object)
            await gistManager.UpdateGistVmStarredStatusAsync(SelectedGistViewModel, updatedGist);

            // Finally try and re-select the original GistFile
            if (gistFilename != null)
            {
                SelectedGistFileViewModel = SelectedGistViewModel.GistFiles.Where(gf => gf.Filename == gistFilename).FirstOrDefault();
            }

            UpdateStatusBar(StatusImage.GitOperation, "Gist Saved.", false);
        }

        private async Task DeleteGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Deleting Gist", true);

            await gistManager.DeleteGistAsync(SelectedGistViewModel);

            gists.Remove(SelectedGistViewModel);

            UpdateStatusBar(StatusImage.Success, "Gist deleted", false);
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

            if (Gists.Count > 0 )
            {
                SelectedGistViewModel = Gists[0];
                SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];
            }

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

        internal void FilenameResetMsgToUser()
        {
            UpdateStatusBar(StatusImage.Warning, "New Filename not unique in Gist. Reset.", false);
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
