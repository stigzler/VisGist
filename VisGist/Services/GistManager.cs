using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Octo = Octokit;
using VisGistModel = VisGist.Models;

namespace VisGist.Services
{
    internal class GistManager
    {

        private GistClientService gitClientService;

        public GistManager(GistClientService gitClientService)
        {
            this.gitClientService = gitClientService;
        }

        internal async Task<ObservableCollection<VisGistModel.Gist>> LoadGistsAsync()
        {
            ObservableCollection<VisGistModel.Gist> gistList = new ObservableCollection<VisGistModel.Gist>();

            IReadOnlyList<Octo.Gist> gistsList = await gitClientService.GetAllGistsAsync();

            foreach (Octo.Gist octoGist in gistsList)
            {
                VisGistModel.Gist gist = new VisGistModel.Gist(octoGist);
                gist.Starred = await gitClientService.GistIsStarredAsync(gist.Id);
                gistList.Add(gist);
            }

            return gistList;
        }



    }
}
