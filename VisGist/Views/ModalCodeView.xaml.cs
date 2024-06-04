using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Themes.MaterialLight.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisGist.ViewModels;

namespace VisGist.Views
{
    /// <summary>
    /// Interaction logic for ModalCodeView.xaml``
    /// </summary>
    public partial class ModalCodeView : ModalDialogBase
    {
        ModalCodeViewModel modalCodeViewModel;

        internal ModalCodeView(ModalCodeViewModel modalCodeViewModel)
        {
            InitializeComponent();

            // Set Theme
            SetTheme(Helpers.UI.IsDarkMode());

            this.modalCodeViewModel = modalCodeViewModel;

            DataContext = modalCodeViewModel;


        }
        private void SetTheme(bool darkMode)
        {
            if (darkMode)
            {
                // Syncfusion Theme operations
                MaterialDarkThemeSettings themeSettings = new MaterialDarkThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialDark.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("MaterialDark", themeSettings);
                SfSkinManager.SetTheme(this, new Theme("MaterialDark"));
            }
            else
            {
                // Syncfusion Theme operations
                MaterialLightThemeSettings themeSettings = new MaterialLightThemeSettings();
                themeSettings.Palette = Syncfusion.Themes.MaterialLight.WPF.MaterialPalette.Blue;
                SfSkinManager.RegisterThemeSettings("MaterialLight", themeSettings);
                SfSkinManager.SetTheme(this, new Theme("MaterialLight"));
            }
        }

        private void ModalDialogBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.CodeWindowSize = new System.Drawing.Size((int)ActualWidth, (int)ActualHeight);
            Properties.Settings.Default.Save();
        }
    }
}
