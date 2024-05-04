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

        /// <summary>
        /// This is either the Gist imported from Gists, or the updated Gist following Gist Save
        /// Used in tracking changes
        /// </summary>
        public Octokit.GistFile ReferenceGistFile { get; set; }
        public GistViewModel ParentGistViewModel { get; set; }
        public string Content { get; set; }
        public string Filename { get => filename; set => SetProperty(ref filename, value); }


        public GistFileViewModel(Octokit.GistFile gistFile, GistViewModel parentGistViewModel)
        {
            ReferenceGistFile = gistFile;
            Content = ReferenceGistFile.Content;
            Filename = ReferenceGistFile.Filename;
            ParentGistViewModel = parentGistViewModel;
        }
    }
}
