using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisGist.Controls
{
    internal class ToolBar: System.Windows.Controls.ToolBar
    {

        internal enum OverflowMode
        {
            AlwaysHidden,
            ShowWhenNeeded,
            AlwaysShow
        }

        public OverflowMode OverflowBehaviour { get; set; } = OverflowMode.ShowWhenNeeded;

        public ToolBar()
        {
            this.Loaded += ToolBar_Loaded; // no overload :(
        }

        private void ToolBar_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OverflowBehaviour != OverflowMode.AlwaysHidden) return;

            if (this.Template.FindName("OverflowGrid", this) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            if (this.Template.FindName("MainPanelBorder", this) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (OverflowBehaviour == OverflowMode.ShowWhenNeeded)
            {
                var overflowGrid = this.Template.FindName("OverflowGrid", this) as FrameworkElement;
                if (overflowGrid != null)
                {
                    overflowGrid.Visibility = this.HasOverflowItems ? Visibility.Visible : Visibility.Collapsed;
                }

                var mainPanelBorder = this.Template.FindName("MainPanelBorder", this) as FrameworkElement;
                if (mainPanelBorder != null)
                {
                    var defaultMargin = new Thickness(0, 0, 11, 0);
                    mainPanelBorder.Margin = this.HasOverflowItems ? defaultMargin : new Thickness(0);
                }
            }

            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
