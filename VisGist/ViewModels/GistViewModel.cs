using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.ViewModels
{
    internal class GistViewModel : ViewModelBase
    {

        private BindingList<GistFileViewModel> gistFiles = new();
        public Octokit.Gist ImportedGist { get; set; }
        public string Id { get => ImportedGist.Id; }
        public string Description { get; set; }
        public bool Public { get; set; }
        public bool Starred { get; set; }
        public BindingList<GistFileViewModel> GistFiles { get => gistFiles; set => SetProperty(ref gistFiles,value); } 

        public GistViewModel(Octokit.Gist gist)
        {
            ImportedGist = gist;
            Description = gist.Description;
            Public = gist.Public;

            foreach (KeyValuePair<string, Octokit.GistFile> fileKvp in gist.Files)
            {
                GistFiles.Add(new GistFileViewModel(fileKvp.Value));
            }

            GistFiles.ListChanged += GistFiles_ListChanged;

        }

        private void GistFiles_ListChanged(object sender, ListChangedEventArgs e)
        {
            GistFiles = new BindingList<GistFileViewModel>(GistFiles.OrderBy(gf => gf.Filename).ToList());
            OnPropertyChanged(nameof(GistFiles));
        }
    }
}
