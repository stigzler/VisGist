using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.ViewModels
{
    internal partial class ModalCodeViewModel : ViewModelBase
    {


        #region Properties =========================================================================================

        //  ==============================================================================================
        //  Backing vars
        private GistFileViewModel selectedGistFileViewModel;
        private Languages selectedLanguage;

        //  ==============================================================================================
        //  Public Members

        public GistFileViewModel SelectedGistFileViewModel { get => selectedGistFileViewModel; set => SetProperty(ref selectedGistFileViewModel, value); }
        public Languages SelectedLanguage { get => selectedLanguage; set => SetProperty(ref selectedLanguage, value); }

        #endregion End: Properties

        public ModalCodeViewModel(GistFileViewModel selectedGistFileViewModel, Languages selectedLanguage)
        {
            this.selectedGistFileViewModel = selectedGistFileViewModel;
            this.selectedLanguage = selectedLanguage;
        }

        public ModalCodeViewModel()
        {                
        }

    }
}
