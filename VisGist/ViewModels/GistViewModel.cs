using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.ViewModels
{
    internal class GistViewModel : ViewModelBase
    {

        private ObservableCollection<GistFileViewModel> gistFiles = new();
        public Octokit.Gist ImportedGist { get; set; }
        public string Id { get => ImportedGist.Id; }
        public string Description { get; set; }
        public bool Public { get; set; }
        public bool Starred { get; set; }
        public ObservableCollection<GistFileViewModel> GistFiles { get => gistFiles; set => SetProperty(ref gistFiles,value); } 

        public GistViewModel(Octokit.Gist gist)
        {
            ImportedGist = gist;
            Description = gist.Description;
            Public = gist.Public;

            foreach (KeyValuePair<string, Octokit.GistFile> fileKvp in gist.Files)
            {
                GistFiles.Add(new GistFileViewModel(fileKvp.Value));
            }
        }

    }
}
