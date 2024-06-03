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
    /// Interaction logic for ModalDialogView.xaml
    /// </summary>
    public partial class ModalDialogView : ModalDialogBase
    {
        ModalDialogViewModel modalDialogViewMdel;

        public ModalDialogView(ModalDialogViewModel modalDialogViewModel)
        {
            InitializeComponent();

            modalDialogViewMdel = modalDialogViewModel;

            this.DataContext = modalDialogViewMdel;
        }
    }
}
