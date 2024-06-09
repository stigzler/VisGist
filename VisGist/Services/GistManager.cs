using Octokit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VisGist.ViewModels;

namespace VisGist.Services
{
    internal class GistManager
    {

        private GistClientService gitClientService;

        public GistManager(GistClientService gitClientService)
        {
            this.gitClientService = gitClientService;
        }


        internal GistFileViewModel CreateNewGistFile(GistViewModel gistVm)
        {
            string dateTime = DateTime.Now.ToString();

            GistFile gistFile = new GistFile(size: 0,
                                            filename: Helpers.String.MakeStringFilenameSafe($"New GistFile created {dateTime}"),
                                            type: "",
                                            language: "",
                                            content: $"New GistFile contents created {dateTime}",
                                            rawUrl: "");
            
            GistFileViewModel gistFileViewModel = new GistFileViewModel(gistFile, gistVm);
            gistFileViewModel.HasChanges = true;
            gistVm.HasChanges = true;
            return gistFileViewModel;
        }

        internal async Task<Gist> SaveGistAsync(GistViewModel gistViewModel)
        {
            // First, update starred status
            await gitClientService.SetGistIsStarredAsync(gistViewModel.Id, gistViewModel.Starred);

            // Construct Gist Update
            GistUpdate gistUpdate = new GistUpdate()
            {
                Description = gistViewModel.Description
            };

            foreach (GistFileViewModel gistFileViewModel in gistViewModel.GistFiles)
            {
                if (gistFileViewModel.MarkedForDeletion) gistFileViewModel.Content = ""; // this essentially deletes the gistfile
                // if not sched for deletion and is blank code, need to add a placeholder so doesn't get deleted
                else if (string.IsNullOrWhiteSpace(gistFileViewModel.Content)) 
                {
                    //gistFileViewModel.Content = $"{{Code blank at last VisGist save on {DateTime.Now}}}";
                    gistFileViewModel.Content = $"{{Blank}}";

                }

                GistFileUpdate gistFileUpdate = new GistFileUpdate()
                {
                    Content = gistFileViewModel.Content,
                    NewFileName = gistFileViewModel.Filename
                };

                gistUpdate.Files.Add(gistFileViewModel.ReferenceGistFile.Filename, gistFileUpdate);
            }

            // Update Gist
            Gist updatedGist = await gitClientService.EditGistAsync(gistViewModel.Id, gistUpdate);

            // Now update Public/Private

            // Now update Starred

            return updatedGist;

        }

        internal async Task<ObservableCollection<GistViewModel>> LoadGistsAsync()
        {
            ObservableCollection<GistViewModel> gistList = new ObservableCollection<GistViewModel>();

            // Note: the below return all details apart from the actual code (GistFile.Contents)
            // You have to do a separate gistClient.Get(gistId) to get the full gist
            IReadOnlyList<Gist> gistsSummaryList = await gitClientService.GetAllGistsAsync();

            foreach (Gist summaryGist in gistsSummaryList)
            {
                // Get full Gist details, including code
                Gist gist = await gitClientService.GetGistAsync(summaryGist.Id);

                GistViewModel gistVM = new GistViewModel(gist);
                //gistVM.ReferenceStarred = await gitClientService.GistIsStarredAsync(gist.Id);
                //gistVM.Starred = gistVM.ReferenceStarred;
                await UpdateGistVmStarredStatusAsync(gistVM, gist);
                gistList.Add(gistVM);
            }

            gistList = new ObservableCollection<GistViewModel>(gistList.OrderBy(x => x.Public));
            return gistList;
        }

        internal async Task UpdateGistVmStarredStatusAsync(GistViewModel gistVM, Gist gist)
        {
            gistVM.ReferenceStarred = await gitClientService.GistIsStarredAsync(gist.Id);
            gistVM.Starred = gistVM.ReferenceStarred;
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
