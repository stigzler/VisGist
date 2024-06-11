using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VisGist.ViewModels;

namespace VisGist.Views
{
    /// <summary>
    /// Interaction logic for ModalCodeView.xaml``
    /// </summary>
    public partial class ModalCodeView : ModalDialogBase
    {
        private ModalCodeViewModel modalCodeViewModel;

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
                CodeEditor.TextArea.TextView.LinkTextForegroundBrush =
                   new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 86, 156, 214));
            }
            else
            {
                CodeEditor.TextArea.TextView.LinkTextForegroundBrush = System.Windows.Media.Brushes.DarkBlue;
            }
        }

        private void ModalDialogBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.CodeWindowSize = new System.Drawing.Size((int)ActualWidth, (int)ActualHeight);
            Properties.Settings.Default.Save();
        }

        private void CodeEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0 && CodeEditor.FontSize < 200)
                    CodeEditor.FontSize += 1;
                else if (CodeEditor.FontSize > 4)
                    CodeEditor.FontSize -= 1;
                e.Handled = true;
            }
        }
    }
}