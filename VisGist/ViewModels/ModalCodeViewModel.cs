using CommunityToolkit.Mvvm.ComponentModel;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.CodeDom;
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
        private string windowTitle = "VisGist Code Preview";
        private IHighlightingDefinition selectedSyntax;
        private bool codeNumberingVisible = false;
        private bool codeWordWrapEnabled = false;

        //  ==============================================================================================
        //  Public Members

        public GistFileViewModel SelectedGistFileViewModel { get => selectedGistFileViewModel; set => SetProperty(ref selectedGistFileViewModel, value); }
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, value); }
        public IHighlightingDefinition SelectedSyntax { get => selectedSyntax; set => SetProperty(ref selectedSyntax, value); }
        public bool CodeNumberingVisible { get => codeNumberingVisible; set => SetProperty(ref codeNumberingVisible, value); }
        public bool CodeWordWrapEnabled { get => codeWordWrapEnabled; set => SetProperty(ref codeWordWrapEnabled, value); }

        #endregion End: Properties

        //public ModalCodeViewModel(GistFileViewModel selectedGistFileViewModel, Languages selectedLanguage)
        //{
        //    this.selectedGistFileViewModel = selectedGistFileViewModel;
        //    this.selectedLanguage = selectedLanguage;
        //}

        public ModalCodeViewModel()
        {
        }

    }
}
