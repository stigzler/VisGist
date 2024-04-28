using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Models
{
    internal class Gist
    {
        public Octokit.Gist ImportedGist { get; set; }

        public string Description { get; set; }

        public bool Public { get; set; }
        public ObservableCollection<GistFile> GistFiles { get; set; } = new();

        public Gist(Octokit.Gist gist)
        {
            ImportedGist = gist;
            Description = gist.Description;
            Public = gist.Public;

            foreach (KeyValuePair<string,Octokit.GistFile> fileKvp in gist.Files)
            {
                GistFiles.Add(new GistFile(fileKvp.Value));
            }

        }

    }
}
