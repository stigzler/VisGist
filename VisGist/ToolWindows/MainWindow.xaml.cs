using Microsoft.VisualStudio.PlatformUI;
using Octokit;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Themes.MaterialLight.WPF;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VisGist.Utility;
using VisGist.ViewModels;

namespace VisGist
{
    public partial class MainWindow : UserControl
    {
        private MainWindowViewModel mainWindowVM;

        // below not used at present, but may be needed
        // private ResourceDictionaryManager resourceDictionaryManager = new ResourceDictionaryManager();


        internal MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            mainWindowVM = (MainWindowViewModel)this.DataContext;

            SetupVmEventHooks();

        }

        private void SetupVmEventHooks()
        {
            mainWindowVM.PropertyChanged += MainWindowVM_PropertyChanged;
        }

        private void MainWindowVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.IsDarkMode):
                    SetTheme(mainWindowVM.IsDarkMode);
                    break;
            }
        }

        private async void TestBT_ClickAsync(object sender, RoutedEventArgs e)
        {
            var options = await General.GetLiveInstanceAsync();
            Debug.WriteLine(options.PersonalAccessToken);


            var client = new GitHubClient(new ProductHeaderValue("VisGit-Tests"));
            var tokenAuth = new Credentials("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"); // NOTE: not real token
            client.Credentials = tokenAuth;

            var user = await client.User.Get("stigzler");

            Debug.WriteLine("{0} has {1} public repositories - go check out their profile at {2}",
                user.Name,
                user.PublicRepos,
                user.Url);

            var gists = await client.Gist.GetAllForUser("stigzler");
        }

        private void SetTheme(bool darkMode)
        {
            if (darkMode)
            {
                // below not used at present, but may be needed
                // resourceDictionaryManager.ChangeTheme(new Uri("pack://application:,,,/VisGist;component/Resources/Themes/Dark.xaml"), this);

                // Syncfusion Theme operations
                MaterialDarkThemeSettings themeSettings = new MaterialDarkThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialDark.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("MaterialDark", themeSettings);
                SfSkinManager.SetTheme(this, new Theme("MaterialDark"));
            }
            else
            {
                // below not used at present, but may be needed
                // resourceDictionaryManager.ChangeTheme(new Uri("pack://application:,,,/VisGist;component/Resources/Themes/Light.xaml"), this);

                // Syncfusion Theme operations
                SfSkinManager.SetTheme(this, new Theme("MaterialLight"));
                MaterialLightThemeSettings themeSettings = new MaterialLightThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialLight.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("FluentDark", themeSettings);
            }
        }
    }
}