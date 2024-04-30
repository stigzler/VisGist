using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using VisGist.ViewModels;
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

        internal async Task<ObservableCollection<GistViewModel>> LoadGistsAsync()
        {
            ObservableCollection<GistViewModel> gistList = new ObservableCollection<GistViewModel>();

            IReadOnlyList<Gist> gistsList = await gitClientService.GetAllGistsAsync();

            foreach (Gist octoGist in gistsList)
            {
                GistViewModel gistVM = new GistViewModel(octoGist);
                gistVM.Starred = await gitClientService.GistIsStarredAsync(octoGist.Id);
                gistList.Add(gistVM);
            }

            return gistList;
        }



    }
}
