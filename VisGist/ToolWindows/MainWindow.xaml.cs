using Microsoft.VisualStudio.PlatformUI;
using Octokit;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.FluentDark.WPF;
using Syncfusion.Themes.FluentLight.WPF;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Themes.MaterialLight.WPF;
using Syncfusion.Themes.Windows11Dark.WPF;
using Syncfusion.Themes.Windows11Light.WPF;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VisGist.ViewModels;

namespace VisGist
{
    public partial class MainWindow : UserControl
    {
        internal MainWindowViewModel MainWindowVM;



        internal MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowVM = mainWindowViewModel;
            this.DataContext = MainWindowVM;

            // THEME
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged; ;

            SetTheme();
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            SetTheme();
        }

        private void SetTheme()
        {
            if (MainWindowVM.IsDarkMode)
            {
                MaterialDarkThemeSettings themeSettings = new MaterialDarkThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialDark.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("MaterialDark", themeSettings);
                SfSkinManager.SetTheme(this, new Theme("MaterialDark"));
            }
            else
            {
                SfSkinManager.SetTheme(this, new Theme("MaterialLight"));
                MaterialLightThemeSettings themeSettings = new MaterialLightThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialLight.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("FluentDark", themeSettings);
            }
        }





        private async void TestBT_Click(object sender, RoutedEventArgs e)
        {
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



        private void SettingsBT_Click(object sender, RoutedEventArgs e)
        {
            var settingsBT = sender as FrameworkElement;
            if (settingsBT != null)
            {
                settingsBT.ContextMenu.IsOpen = true;
            }
        }
    }
}