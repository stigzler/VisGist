using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VisGist.ViewModels;

namespace VisGist
{
    public class MyToolWindow : BaseToolWindow<MyToolWindow>
    {
        internal MainWindowViewModel MainWindowViewModel = new MainWindowViewModel();


        public override string GetTitle(int toolWindowId) => "Gists";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new MainWindow(MainWindowViewModel));
        }

        [Guid("1baca796-1bb3-41d1-b804-adeb9f3018cc")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}