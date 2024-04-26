using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.ViewModels
{
    internal class MainWindowViewModel: ViewModelBase
    {
        public bool IsDarkMode { get => Helpers.UI.IsDarkMode(); }
        public bool IsAuthenticated { get; set; } = false;
        public MainWindowViewModel()
        {
                
        }
    }
}
