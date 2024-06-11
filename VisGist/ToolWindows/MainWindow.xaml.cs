using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

            // Set MainWindowViewModel as data context
            mainWindowVM = (MainWindowViewModel)this.DataContext;

            // Setup any MainWindowViewModel event handlers
            SetupVmEventHooks();

            // Set Theme
            SetTheme(Helpers.UI.IsDarkMode());

            // Do any UI Layout tasks
            GridLengthConverter converter = new GridLengthConverter();
            GistDescriptionRow.Height = (GridLength)converter.ConvertFromString(Properties.Settings.Default.SplitterGistDescription);
            GistBrowserRow.Height = (GridLength)converter.ConvertFromString(Properties.Settings.Default.SplitterGistTree);

            // SyntaxHighlighting Tests
        }

        private void SetupVmEventHooks()
        {
            mainWindowVM.PropertyChanged += MainWindowVM_PropertyChanged;
            mainWindowVM.FilenameChangeDetected += MainWindowVM_FilenameChangeDetected;
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
                // Below sets the hyperlink color in avalonEdit, as this cannot be changed via the xhsd files
                CodeEditor.TextArea.TextView.LinkTextForegroundBrush =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 86, 156, 214));
            }
            else
            {
                // Below sets the hyperlink color in avalonEdit, as this cannot be changed via the xhsd files
                CodeEditor.TextArea.TextView.LinkTextForegroundBrush = System.Windows.Media.Brushes.DarkBlue;
            }
        }

        private void MyToolWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight > ActualWidth) mainWindowVM.LayoutHorizontal = true;
            else mainWindowVM.LayoutHorizontal = false;
        }

        // yeah - I know, I know, But might be dead tomorrow.
        private void MainWindowVM_FilenameChangeDetected(bool unique)
        {
            if (unique)
            {
                GistFileFilenameTB.BorderBrush = new SolidColorBrush(Colors.Red);
                GistFileFilenameTB.BorderThickness = new Thickness(1);
            }
            else
            {
                GistFileFilenameTB.BorderThickness = new Thickness(0);
            }
        }

        private void GistsTV_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void GistsTV_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SaveUiLayoutPersistance();
        }

        private void GistDescriptionSV_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SaveUiLayoutPersistance();
        }

        private void SaveUiLayoutPersistance()
        {
            GridLengthConverter converter = new GridLengthConverter();
            Properties.Settings.Default.SplitterGistTree = converter.ConvertToString(GistBrowserRow.Height);
            Properties.Settings.Default.SplitterGistDescription = converter.ConvertToString(GistDescriptionRow.Height);
            Properties.Settings.Default.Save();
        }
        private void GistFileFilenameTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { CodeEditor.Focus(); e.Handled = true; }
        }

        private void CodeEditor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // ZOOMs text in code Editor on CTRL+MouseWheel
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