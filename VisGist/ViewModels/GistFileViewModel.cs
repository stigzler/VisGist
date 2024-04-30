using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.ViewModels
{
    internal class GistFileViewModel : ViewModelBase
    {
        private string filename;

        public Octokit.GistFile ImportedGistFile { get; set; }

        public string Content { get; set; }

        public string Filename { get => filename; set => SetProperty(ref filename, value); }

        public string Language { get; set; }

        public GistFileViewModel(Octokit.GistFile gistFile)
        {
            ImportedGistFile = gistFile;
            Content = ImportedGistFile.Content;
            Filename = ImportedGistFile.Filename;
            Language = ImportedGistFile.Language;
        }
    }
}
