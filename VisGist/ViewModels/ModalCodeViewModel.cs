﻿using CommunityToolkit.Mvvm.ComponentModel;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
        private int codeSize;


        //  ==============================================================================================
        //  Public Members

        public GistFileViewModel SelectedGistFileViewModel { get => selectedGistFileViewModel; set => SetProperty(ref selectedGistFileViewModel, value); }
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, value); }
        public IHighlightingDefinition SelectedSyntax { get => selectedSyntax; set => SetProperty(ref selectedSyntax, value); }
        public bool CodeNumberingVisible { get => codeNumberingVisible; set => SetProperty(ref codeNumberingVisible, value); }
        public bool CodeWordWrapEnabled { get => codeWordWrapEnabled; set => SetProperty(ref codeWordWrapEnabled, value); }
        public int CodeSize { get => codeSize; set => SetProperty(ref codeSize, value); }

        #endregion Properties =========================================================================================

        public ModalCodeViewModel() 
        { 
            
        }
     
    }
}