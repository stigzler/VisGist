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
                gistVM.ReferenceStarred = await gitClientService.GistIsStarredAsync(octoGist.Id);
                gistVM.Starred = gistVM.ReferenceStarred;
                gistList.Add(gistVM);
            }
            gistList = new ObservableCollection<GistViewModel>(gistList.OrderBy(x => x.Public));
            return gistList;
        }

        internal async Task<GistViewModel> CreateNewGistAsync(bool @public)
        {
            string dateTime = DateTime.Now.ToString();

            Gist gist = await gitClientService.CreateNewGistAsync(@public, 
                $"New Gist created {dateTime}",
                $"New Gist created {dateTime}",
                $"File created by VisGist on {dateTime}");

            GistViewModel gistViewModel = new GistViewModel(gist);

            return gistViewModel;
        }

        internal async Task DeleteGistAsync(GistViewModel gistViewModel)
        {
            await gitClientService.DeleteGistAsync(gistViewModel.Id);
        }



    }
}
