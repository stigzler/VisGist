using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using VisGist.Enums;
using VisGist.Services;
using Languages = Syncfusion.Windows.Edit.Languages;
using Octokit;
using System.Media;
using System.ComponentModel;
using System.Windows.Data;
using Syncfusion.Windows.Shared;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.VCProjectEngine;
using VisGist.Data;
using VisGist.Views;
using System.Diagnostics;
using VisGist.Models;

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
        private string searchExpression = String.Empty;
        private GistSortMethod sortMethod = GistSortMethod.Alphabetical;

        private ObservableCollection<GistViewModel> collatedGists = new ObservableCollection<GistViewModel>();
        private ObservableCollection<GistViewModel> allGists = new ObservableCollection<GistViewModel>();


        private ViewModelBase selectedGistVmItem;
        private GistViewModel selectedGistViewModel;
        private GistFileViewModel selectedGistFileViewModel;

        private GridResizeDirection browserEditorsSplitterDirection = GridResizeDirection.Rows;
        private bool layoutHorizontal = false;
        private System.Windows.Media.FontFamily codeFont = new System.Windows.Media.FontFamily("Consolas");

        // Public members
        public bool IsDarkMode { get => isDarkMode; set => SetProperty(ref isDarkMode, value); }
        public bool IsAuthenticated { get => isAuthenticated; set => SetProperty(ref isAuthenticated, value); }
        public bool ViewLoaded { get; set; } = false;
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
        public ObservableCollection<GistViewModel> CollatedGists { get => collatedGists; set => SetProperty(ref collatedGists, value); }
        public ObservableCollection<GistViewModel> AllGists { get => allGists; set => SetProperty(ref allGists, value); }
        public ICollectionView GistsView { get => CollectionViewSource.GetDefaultView(CollatedGists); }
        public ViewModelBase SelectedGistVmItem
        {
            get { return selectedGistVmItem; }
            set
            {
                SetProperty(ref selectedGistVmItem, value);
                OnSelectedGistItemChanged(value);
            }
        }

        public System.Windows.Media.FontFamily CodeFont
        {
            get
            {
                return userVsOptions.CodeFont;
            }
        }
        public string SearchExpression //{ get => searchExpression; set => { SetProperty(ref searchExpression, value); GistsView.Refresh(); } }
        {
            get { return searchExpression; }
            set { SetProperty(ref searchExpression, value); SortAndSerachGists(); }
        }

        public GistSortMethod SortMethod
        {
            get { return sortMethod; }
            set { SetProperty(ref sortMethod, value); SortAndSerachGists(); }
        }

        // Commands ----------------------------------------------------------------------------------------------

        //public IRelayCommand SetCodeNumberingVisibleCMD { get => setCodeNumberingVisibleCMD; }

        #endregion End: PROPERTIES ---------------------------------------------------------------------------------

        #region OPERATIONAL PRIVATE VARS ===========================================================================

        private GistClientService gitClientService = new GistClientService();
        private GistManager gistManager;
        private General userVsOptions = new General();
        //private CodeEditorManager codeEditorManager;

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
        public IRelayCommand SortGistsCMD { get; set; }
        public IRelayCommand MakeGistTitleCMD { get; set; }
        public IRelayCommand CollapseTreeCMD { get; set; }

        public IRelayCommand PopoutCodeCMD { get; set; }


        #endregion End: COMMANDS ---------------------------------------------------------------------------------


        // ==============================================================================================
        // CONSTRUCTOR
        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            // Setup Managers
            gistManager = new GistManager(gitClientService);

            SetupCommands();


        }

        private void CollapseTree(TreeView treeView)
        {
            foreach (GistViewModel tvi in treeView.Items)
            {
                TreeViewItem treeItem = treeView.ItemContainerGenerator.ContainerFromItem(tvi) as TreeViewItem;
                if (treeItem != null)
                    treeItem.IsExpanded = false;
            }
        }

        private void SortAndSerachGists()
        {
            //return SearchExpression == null
            //    || gist.PrimaryGistFilename.IndexOf(SearchExpression, StringComparison.OrdinalIgnoreCase) != -1;

            if (AllGists.Count > 0)
            {
                // Update the existing instance to significantly improve the responsiveness of the UI
                CollatedGists.Clear();
                SelectedGistViewModel = null;
                SelectedGistFileViewModel = null;

                ObservableCollection<GistViewModel> sortedGists = new ObservableCollection<GistViewModel>();

                switch (sortMethod)
                {
                    case GistSortMethod.Alphabetical:
                        sortedGists = new ObservableCollection<GistViewModel>(AllGists.OrderBy(g => g.PrimaryGistFilename));
                        break;
                    case GistSortMethod.Starred:
                        sortedGists = new ObservableCollection<GistViewModel>(AllGists.OrderByDescending(g => g.Starred));
                        break;
                    case GistSortMethod.Public:
                        sortedGists = new ObservableCollection<GistViewModel>(AllGists.OrderBy(g => g.Public));
                        break;
                }

                foreach (GistViewModel gistVm in sortedGists)
                {
                    gistVm.NodeExpanded = false;
                    if (gistVm.PrimaryGistFilename.ToLower().Contains(SearchExpression.ToLower()))
                    {
                        if (!CollatedGists.Any(cg => cg.Id == gistVm.Id)) CollatedGists.Add(gistVm);
                    }

                    if (!SearchExpression.IsNullOrWhiteSpace()
                        && gistVm.GistFiles.Any(gf => gf.Filename.ToLower().Contains(SearchExpression.ToLower())))
                    {
                        if (!CollatedGists.Any(cg => cg.Id == gistVm.Id)) CollatedGists.Add(gistVm);
                        gistVm.NodeExpanded = true;

                    }
                }

                if (CollatedGists.Count > 0)
                {
                    SelectedGistViewModel = CollatedGists[0];
                    SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];
                }
            }
        }

        private async Task OnViewLoadedAsync()
        {
            userVsOptions = await General.GetLiveInstanceAsync();

            if (userVsOptions.PersonalAccessToken.IsNullOrWhiteSpace())
                UpdateStatusBar(StatusImage.Warning, "Please set Access Token in Options>VisGist", false);
            else
                await AuthenticateAndLoadGistsAsync();
        }

        private async Task AuthenticateAndLoadGistsAsync()
        {
            await AuthenticateUserAsync();
            if (IsAuthenticated) await GetAllGistsAsync();
        }

        private void SetupCommands()
        {
            GitAuthenticateCMD = new AsyncRelayCommand(AuthenticateAndLoadGistsAsync);
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
            SortGistsCMD = new RelayCommand<GistSortMethod>((param) => SortGists(param));
            MakeGistTitleCMD = new RelayCommand(MakeGistTitle);
            CollapseTreeCMD = new RelayCommand<TreeView>(CollapseTree);
            PopoutCodeCMD = new RelayCommand(PopoutCode);
        }

        private void PopoutCode()
        {
            //ModalDialogViewModel modalDialogViewModel = new ModalDialogViewModel();
            //modalDialogViewModel.Button1Text = button1Text;
            //modalDialogViewModel.Button2Text = button2Text;
            //modalDialogViewModel.DialogText = dialogText;
            //modalDialogViewModel.WindowTitle = windowTitle;

            //ModalDialogView modalDialog = new ModalDialogView(modalDialogViewModel);
            //modalDialog.ShowModal();

            ModalCodeViewModel modalCodeViewModel = new ModalCodeViewModel();

            ModalCodeView modalCodeView = new ModalCodeView(modalCodeViewModel);
            modalCodeViewModel.SelectedGistFileViewModel = SelectedGistFileViewModel;
            modalCodeViewModel.WindowTitle = "VisGist Code: " + SelectedGistFileViewModel.Filename;

            if (SelectedLanguage == Syncfusion.Windows.Edit.Languages.Text)
                modalCodeViewModel.SelectedLanguage = Syncfusion.Windows.Edit.Languages.Custom;
            else
                modalCodeViewModel.SelectedLanguage = SelectedLanguage;



            modalCodeView.ShowModal();



            //ModalDialogView modalDialog = new ModalDialogView(modalDialogViewModel);
            //modalDialog.ShowModal();

        }

        private void MakeGistTitle()
        {
            char headingChar = Constants.GistFileHeadingChars[userVsOptions.GistFileHeadingCharacter];

            foreach (GistFileViewModel gistFileViewModel in SelectedGistViewModel.GistFiles)
            {
                if (gistFileViewModel.Filename[0] == headingChar)
                    gistFileViewModel.Filename = gistFileViewModel.Filename.Remove(0, 1);

                //gistFileViewModel.Filename.Replace(headingChar, "");
            }

            SelectedGistFileViewModel.Filename = headingChar + SelectedGistFileViewModel.Filename;

        }

        private void SortGists(GistSortMethod gistSortMethod)
        {
            SortMethod = gistSortMethod;
        }

        private async Task DeleteGistFileAsync()
        {
            if (SelectedGistViewModel.GistFiles.Count == 1)
            {
                SystemSounds.Exclamation.Play();
                UpdateStatusBar(StatusImage.Warning, "Cannot delete last file. Delete Gist instead.", false);
                return;
            }

            if (userVsOptions.ConfirmDelete)
            {
                string dialogRepsonse = GetDialogRepsonse("Delete GistFile?", $"Delete GistFile \"{SelectedGistFileViewModel.Filename}\"?", "Yes", "No");
                if (dialogRepsonse != "Yes") return;
            }

            SelectedGistFileViewModel.MarkedForDeletion = true;
            await SaveGistAsync();
            SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];
            SelectedGistViewModel.RefreshPrimaryGistFilename();

        }

        private async Task AddNewGistFileAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Adding New GistFile", true);
            GistFileViewModel gistFileViewModel = gistManager.CreateNewGistFile(SelectedGistViewModel);
            SelectedGistViewModel.AddGistFile(gistFileViewModel);
            await SaveGistAsync();
            SelectedGistViewModel.SortGistFiles();
            UpdateStatusBar(StatusImage.Success, "New GistFile added successfully", false);
        }

        private async Task AddNewGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, "Adding New Gist", true);

            GistViewModel gistViewModel = await gistManager.CreateNewGistAsync(userVsOptions.NewGistPublic);

            collatedGists.Insert(0, gistViewModel);

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
            Octokit.Gist updatedGist = await gistManager.SaveGistAsync(SelectedGistViewModel);

            // Now update the GistViewModel (populates any properties figured server side like filesize etc)
            SelectedGistViewModel.UpdateGistFile(updatedGist);

            // Now update starred status of GistViewModel (as this isn't stored in the Gist data object)
            await gistManager.UpdateGistVmStarredStatusAsync(SelectedGistViewModel, updatedGist);

            // Finally try and re-select the original GistFile
            if (gistFilename != null)
            {
                SelectedGistFileViewModel = SelectedGistViewModel.GistFiles.Where(gf => gf.Filename == gistFilename).FirstOrDefault();
            }

            UpdateStatusBar(StatusImage.Success, "Gist Saved.", false);
        }

        private async Task SaveSpecificGistAsync(GistViewModel gistViewModel)
        {

        }

        private async Task DeleteGistAsync()
        {
            if (userVsOptions.ConfirmDelete)
            {
                string dialogRepsonse = GetDialogRepsonse("Delete Gist?", $"Delete Gist \"{SelectedGistViewModel.PrimaryGistFilename}\"?", "Yes", "No");
                if (dialogRepsonse != "Yes") return;
            }

            UpdateStatusBar(StatusImage.GitOperation, "Deleting Gist", true);

            await gistManager.DeleteGistAsync(SelectedGistViewModel);

            collatedGists.Remove(SelectedGistViewModel);

            UpdateStatusBar(StatusImage.Success, "Gist deleted", false);
        }
        private void OnSelectedGistItemChanged(ViewModelBase gistItem)
        {
            if (gistItem is GistViewModel)
            {
                SelectedGistViewModel = (GistViewModel)gistItem;

                if (SelectedGistViewModel.GistFiles.Count() > 0)
                    SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];

            }
            else if (gistItem is GistFileViewModel)
            {
                SelectedGistFileViewModel = (GistFileViewModel)gistItem;
                SelectedGistViewModel = SelectedGistFileViewModel.ParentGistViewModel;
                if (userVsOptions.AutoLanguageSelect)
                {
                    SelectedLanguage = Helpers.String.FilenameToLanguage(SelectedGistFileViewModel.Filename);
                }
            }
        }

        private async Task GetAllGistsAsync()
        {
            
            if (AllGists.Any(g => g.HasChanges))
            {
                string dialogRepsonse = GetDialogRepsonse("Load Gists from Github?", $"There are unsaved changes. Loading Gists from Github will overwrite these. Are you sure you want to proceed?", "Yes", "No");
                if (dialogRepsonse != "Yes") return;
            }


            UpdateStatusBar(StatusImage.GitOperation, $"Loading Gists..", true);

            AllGists = await gistManager.LoadGistsAsync();

            SortMethod = GistSortMethod.Alphabetical;

            //if (AllGists.Count > 0)
            //{
            //    // Update the existing instance to significantly improve the responsiveness of the UI
            //    CollatedGists.Clear();
            //    foreach (GistViewModel gist in AllGists)
            //    {
            //        CollatedGists.Add(gist);
            //    }

            //    SelectedGistViewModel = CollatedGists[0];
            //    SelectedGistFileViewModel = SelectedGistViewModel.GistFiles[0];
            //}

            UpdateStatusBar(StatusImage.Success, $"Gists Loaded Successfully", false);
        }

        private async Task DoTestActionAsync()
        {
            ModalDialogViewModel modalDialogViewModel = new ModalDialogViewModel();
            modalDialogViewModel.Button1Text = "Yes";
            modalDialogViewModel.Button2Text = "No";
            modalDialogViewModel.DialogText = @"Are you sure you wish to delete the Gist 'DaveWozEre.md'?";

            ModalDialogView modalDialog = new ModalDialogView(modalDialogViewModel);
            modalDialog.ShowModal();

            Debug.WriteLine(modalDialogViewModel.SelectedButtonText);

            //collatedGists[0].GistFiles[0].Filename = "Zzzzz - aappp!";

            // LayoutHorizontal = !LayoutHorizontal;

            //BrowserEditorsSplitterDirection = GridResizeDirection.Columns;

            //await gitClientService.DoTestActionAsync(this);
        }

        private string GetDialogRepsonse(string windowTitle, string dialogText, string button1Text, string button2Text)
        {
            ModalDialogViewModel modalDialogViewModel = new ModalDialogViewModel();
            modalDialogViewModel.Button1Text = button1Text;
            modalDialogViewModel.Button2Text = button2Text;
            modalDialogViewModel.DialogText = dialogText;
            modalDialogViewModel.WindowTitle = windowTitle;

            ModalDialogView modalDialog = new ModalDialogView(modalDialogViewModel);
            modalDialog.ShowModal();

            return modalDialogViewModel.SelectedButtonText;
        }



        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            if (!ViewLoaded) return;
            IsDarkMode = Helpers.UI.IsDarkMode();
        }



        private async Task AuthenticateUserAsync()
        {
            if (userVsOptions.PersonalAccessToken.IsNullOrWhiteSpace()) return;

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
