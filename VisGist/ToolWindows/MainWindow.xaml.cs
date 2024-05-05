using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Themes.MaterialLight.WPF;
using System.Windows;
using System.Windows.Controls;
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
            // this controls for a strange can't find Microsoft.Xaml.Behaviors library bug
            var trig = new Microsoft.Xaml.Behaviors.EventTrigger(); trig.SourceName = "foo";

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
                SfSkinManager.RegisterThemeSettings("MaterialLight", themeSettings);
            }
        }

        private void MyToolWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > ActualWidth) mainWindowVM.LayoutHorizontal = true;
            else mainWindowVM.LayoutHorizontal = false;
        }
    }
}