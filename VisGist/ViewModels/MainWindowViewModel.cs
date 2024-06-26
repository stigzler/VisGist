﻿// Ignore Spelling: Popout Github Vm

using CommunityToolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using VisGist.Data.Models;
using VisGist.Enums;
using VisGist.Services;
using VisGist.Views;
using Constants = VisGist.Data.Constants;

namespace VisGist.ViewModels
{
    internal partial class MainWindowViewModel : ViewModelBase
    {
        #region PROPERTIES =========================================================================================

        //  PRIVATE BACKING MEMBERS ==============================================================================================

        private bool isDarkMode;
        private bool isAuthenticated = false;
        private StatusImage statusImage = StatusImage.Information;
        private string statusText = "Welcome to VisGist";
        private bool statusBarVisible = false;
        private bool syntaxHighlightingEnabled = false;
        private bool codeNumberingVisible = false;
        private bool codeWordWrapEnabled = false;
        private int codeSize;
        private string searchExpression = String.Empty;
        private GistSortMethod sortMethod = GistSortMethod.Alphabetical;
        private IHighlightingDefinition selectedSyntax = null;

        private ObservableCollection<GistViewModel> collatedGists = new ObservableCollection<GistViewModel>();
        private ObservableCollection<GistViewModel> allGists = new ObservableCollection<GistViewModel>();
        private ObservableCollection<SyntaxViewModel> syntaxes = new ObservableCollection<SyntaxViewModel>();

        private ViewModelBase selectedGistVmItem;
        private GistViewModel selectedGistViewModel;
        private GistFileViewModel selectedGistFileViewModel;
        private SyntaxViewModel selectedSyntaxViewModel = null;

        private GridResizeDirection browserEditorsSplitterDirection = GridResizeDirection.Rows;
        private bool layoutHorizontal = false;
        private System.Windows.Media.FontFamily codeFont = new System.Windows.Media.FontFamily("Consolas");

        //  PUBLIC PROPERTIES ==============================================================================================

        // UI properties
        public bool IsDarkMode { get => isDarkMode; set => SetProperty(ref isDarkMode, value); }

        public bool ViewLoaded { get; set; } = false;
        public bool CodeNumberingVisible { get => codeNumberingVisible; set => SetProperty(ref codeNumberingVisible, value); }
        public bool CodeWordWrapEnabled { get => codeWordWrapEnabled; set => SetProperty(ref codeWordWrapEnabled, value); }
        public int CodeSize { get => codeSize; set => SetProperty(ref codeSize, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public bool StatusBarVisible { get => statusBarVisible; set => SetProperty(ref statusBarVisible, value); }
        public bool SyntaxHighlightingEnabled { get => syntaxHighlightingEnabled; set => SetProperty(ref syntaxHighlightingEnabled, value); }
        public bool LayoutHorizontal { get => layoutHorizontal; set => SetProperty(ref layoutHorizontal, value); }
        public StatusImage StatusImage { get => statusImage; set => SetProperty(ref statusImage, value); }

        // Data Related
        public bool IsAuthenticated { get => isAuthenticated; set => SetProperty(ref isAuthenticated, value); }

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

        // Selections
        public ViewModelBase SelectedGistVmItem
        {
            get { return selectedGistVmItem; }
            set
            {
                SetProperty(ref selectedGistVmItem, value);
                OnSelectedGistItemChanged(value);
            }
        }

        public SyntaxViewModel SelectedSyntaxViewModel
        {
            get { return selectedSyntaxViewModel; }
            set
            {
                SetProperty(ref selectedSyntaxViewModel, value);
                SetSyntaxHighlighting();
            }
        }

        public GistViewModel SelectedGistViewModel { get => selectedGistViewModel; set => SetProperty(ref selectedGistViewModel, value); }
        public GistFileViewModel SelectedGistFileViewModel { get => selectedGistFileViewModel; set => SetProperty(ref selectedGistFileViewModel, value); }
        public IHighlightingDefinition SelectedSyntax { get => selectedSyntax; set => SetProperty(ref selectedSyntax, value); }

        // Collections
        public ObservableCollection<GistViewModel> CollatedGists { get => collatedGists; set => SetProperty(ref collatedGists, value); }

        public ObservableCollection<GistViewModel> AllGists { get => allGists; set => SetProperty(ref allGists, value); }
        public ObservableCollection<SyntaxViewModel> Syntaxes { get => syntaxes; set => SetProperty(ref syntaxes, value); }
         
        #endregion PROPERTIES =========================================================================================

        #region OPERATIONAL PRIVATE VARS ===========================================================================

        private GistClientService gitClientService = new GistClientService();
        private GistManager gistManager;
        private General userVsOptions = new General();
        private SyntaxManager syntaxManager;

        #endregion OPERATIONAL PRIVATE VARS ===========================================================================

        #region EVENTS =========================================================================================

        internal delegate void VSThemeChanged(bool aDarkModeTheme);

        internal event VSThemeChanged vsThemeChanged;

        internal delegate void FilenameChange(bool duplicate);

        internal event FilenameChange FilenameChangeDetected;

        #endregion EVENTS =========================================================================================

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
        public IAsyncRelayCommand SaveAllGistsCMD { get; set; }
        public IRelayCommand SetSyntaxHighlightingCMD { get; set; }
        public IRelayCommand SetCodeNumberingVisibleCMD { get; set; }
        public IRelayCommand SetCodeWordWrapEnabledCMD { get; set; }
        public IRelayCommand SortGistsCMD { get; set; }
        public IRelayCommand MakeGistTitleCMD { get; set; }
        public IRelayCommand CollapseTreeCMD { get; set; }
        public IRelayCommand PopoutCodeCMD { get; set; }
        public IRelayCommand ViewInGithubCMD { get; set; }

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
            DoTestActionCMD = new AsyncRelayCommand<object>(DoTestActionAsync);
            SaveGistCMD = new AsyncRelayCommand(SaveGistAsync);
            SaveAllGistsCMD = new AsyncRelayCommand(SaveAllGistsAsync);
            SetCodeNumberingVisibleCMD = new RelayCommand(SetCodeNumberingVisible);
            SetCodeWordWrapEnabledCMD = new RelayCommand(SetCodeWordWrapEnabled);
            SortGistsCMD = new RelayCommand<GistSortMethod>((param) => SortGists(param));
            MakeGistTitleCMD = new RelayCommand(MakeGistTitle);
            CollapseTreeCMD = new RelayCommand<TreeView>(CollapseTree);
            PopoutCodeCMD = new RelayCommand(PopoutCode);
            ViewInGithubCMD = new RelayCommand(ViewInGithub);
        }

        #endregion COMMANDS =========================================================================================

        #region CONTRUCTORS =========================================================================================

        public MainWindowViewModel()
        {
            // Detect VisualStudio Theme Changes
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            // Set Dark Mode
            IsDarkMode = Helpers.UI.IsDarkMode();

            // Ensure User Syntax files are in User folder - NB: This must precede newing up SyntaxManager.
            Helpers.File.EnsureUserSyntaxFiles();

            // Setup Managers
            gistManager = new GistManager(gitClientService);
            syntaxManager = new SyntaxManager();

            // Now setup Syntaxes
            UpdateSyntaxes(IsDarkMode);

            // Link Commands and User Operation Methods
            SetupCommands();
        }

        #endregion CONTRUCTORS =========================================================================================

        #region USER OPERATIONS =========================================================================================

        private async Task DoTestActionAsync(object obj)
        {
            await Task.CompletedTask;
        }

        private async Task AuthenticateUserAsync()
        {
            if (string.IsNullOrWhiteSpace(userVsOptions.PersonalAccessToken)) return;

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

                    if (!string.IsNullOrWhiteSpace(SearchExpression)
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

        private async Task AuthenticateAndLoadGistsAsync()
        {
            await AuthenticateUserAsync();
            if (IsAuthenticated) await GetAllGistsAsync();
        }

        private void ViewInGithub()
        {
            if (SelectedGistViewModel == null) return;

            System.Diagnostics.Process.Start(SelectedGistViewModel.ReferenceGist.HtmlUrl);
        }

        private void PopoutCode()
        {
            ModalCodeViewModel modalCodeViewModel = new ModalCodeViewModel();

            ModalCodeView modalCodeView = new ModalCodeView(modalCodeViewModel);
            modalCodeViewModel.SelectedGistFileViewModel = SelectedGistFileViewModel;
            modalCodeViewModel.WindowTitle = "VisGist Code: " + SelectedGistFileViewModel.Filename;
            modalCodeViewModel.CodeNumberingVisible = CodeNumberingVisible;
            modalCodeViewModel.CodeWordWrapEnabled = CodeWordWrapEnabled;
            modalCodeViewModel.SelectedSyntax = SelectedSyntax;
            modalCodeViewModel.CodeSize = CodeSize;
            modalCodeView.ShowModal();
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

            AllGists.Insert(0, gistViewModel);

            RefreshGistList();

            UpdateStatusBar(StatusImage.Success, "New Gist added successfully", false);
        }

        private void SetCodeNumberingVisible()
        {
            CodeNumberingVisible = !CodeNumberingVisible;
        }

        private void SetCodeWordWrapEnabled()
        {
            CodeWordWrapEnabled = !CodeWordWrapEnabled;
        }

        private async Task SaveGistAsync()
        {
            UpdateStatusBar(StatusImage.GitOperation, $"Saving: {SelectedGistViewModel.PrimaryGistFilename}", true);

            // store the name of the selected gist file as update essentially creates new gistfile object
            // breaking the link between the UI gistfile and the dataobject one
            string gistFilename = SelectedGistFileViewModel?.Filename;

            await SaveSpecificGistAsync(SelectedGistViewModel);

            // Finally try and re-select the original GistFile
            if (gistFilename != null)
            {
                SelectedGistFileViewModel = SelectedGistViewModel.GistFiles.Where(gf => gf.Filename == gistFilename).FirstOrDefault();
            }

            UpdateStatusBar(StatusImage.Success, "Gist Saved.", false);
        }

        private async Task SaveAllGistsAsync()
        {
            IEnumerable<GistViewModel> changedGists = CollatedGists.Where(cg => cg.HasChanges == true &&
                                                                        cg.CanSave == true && cg.HasErrors == false);

            if (changedGists.Count() == 0)
            {
                UpdateStatusBar(StatusImage.Information, "No Gists need saving.", false);
                return;
            }

            foreach (GistViewModel gistViewModel in changedGists)
            {
                UpdateStatusBar(StatusImage.GitOperation, $"Saving: {gistViewModel.PrimaryGistFilename}", true);
                await SaveSpecificGistAsync(gistViewModel);
            }

            UpdateStatusBar(StatusImage.Success, "Eligible Gists Saved.", false);
        }

        private async Task SaveSpecificGistAsync(GistViewModel gistViewModel)
        {
            // Save (update) gist in git server
            Octokit.Gist updatedGist = await gistManager.SaveGistAsync(gistViewModel);

            // Now update the GistViewModel (populates any properties figured server side like filesize etc)
            gistViewModel.UpdateGistFile(updatedGist);

            // Now update starred status of GistViewModel (as this isn't stored in the Gist data object)
            await gistManager.UpdateGistVmStarredStatusAsync(gistViewModel, updatedGist);
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

            AllGists.Remove(SelectedGistViewModel);

            RefreshGistList();

            UpdateStatusBar(StatusImage.Success, "Gist deleted", false);
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

            UpdateStatusBar(StatusImage.Success, $"Gists Loaded Successfully", false);
        }

        private void LogOut()
        {
            gitClientService.Logout();
            UpdateStatusBar(StatusImage.Information, $"Logged Out Successfully");
            IsAuthenticated = false;
        }

        #endregion USER OPERATIONS =========================================================================================

        #region VIEW LOGIC =========================================================================================

        private void UpdateSyntaxes(bool darkMode)
        {
            Syntaxes.Clear();
            Syntaxes.Add(new SyntaxViewModel() { Name = "None" });
            foreach (Syntax syntax in syntaxManager.Syntaxes)
            {
                if (!darkMode && syntax.FileLightTheme != null) Syntaxes.Add(new SyntaxViewModel(syntax, darkMode));
                else if (darkMode && syntax.FileDarkTheme != null) Syntaxes.Add(new SyntaxViewModel(syntax, darkMode));
            }
        }

        private void SetSyntaxHighlighting()
        {
            if (SelectedSyntaxViewModel == null || SelectedSyntaxViewModel.Name == "None")
            {
                SelectedSyntax = null;
                return;
            }

            using (XmlReader reader = XmlReader.Create(SelectedSyntaxViewModel.Filename))
            {
                SelectedSyntax = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        private async Task OnViewLoadedAsync()
        {
            if (!this.ViewLoaded)
            {
                userVsOptions = await General.GetLiveInstanceAsync();

                if (string.IsNullOrWhiteSpace(userVsOptions.PersonalAccessToken))
                    UpdateStatusBar(StatusImage.Warning, "Please set Access Token in Options>VisGist", false);
                else
                    await AuthenticateAndLoadGistsAsync();

                ViewLoaded = true;

                CodeSize = userVsOptions.DefaultCodeSize;
            }
        }

        private void RefreshGistList()
        {
            SortAndSerachGists();
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
            }

            AutoUpdateCodeSyntax();
        }

        private void AutoUpdateCodeSyntax()
        {
            if (userVsOptions.AutoLanguageSelect && SelectedGistFileViewModel != null)
            {
                SyntaxViewModel matchedSyntax = Syntaxes.Where(s => s.Extensions != null &&
                                    s.Extensions.Contains(Path.GetExtension(SelectedGistFileViewModel.Filename), StringComparer.OrdinalIgnoreCase))
                                    .FirstOrDefault();

                if (matchedSyntax != null)
                    SelectedSyntaxViewModel = Syntaxes.Where(s => s.Name == matchedSyntax.Name).FirstOrDefault();
                else
                    SelectedSyntaxViewModel = null;
            }
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

            // Update DarkMode property
            IsDarkMode = Helpers.UI.IsDarkMode();

            // Update SyntaxViewModels
            UpdateSyntaxes(IsDarkMode);
            Syntaxes.Count();
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

        #endregion VIEW LOGIC =========================================================================================
    }
}