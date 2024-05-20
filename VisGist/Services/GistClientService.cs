using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Services
{
    internal class GistClientService
    {
        private GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(Data.Constants.GitProductHeaderValue));

        public GistClientService()
        {
        }

        internal void Logout()
        {
            gitHubClient.Credentials = new Credentials("NULLED - FORCE LOGOUT ON ANY MORE REQUESTS");
        }
        internal async Task<Exception> AuthenticateAsync()
        {
            var userVsOptions = await General.GetLiveInstanceAsync();

            var tokenAuth = new Credentials(userVsOptions.PersonalAccessToken);
            gitHubClient.Credentials = tokenAuth;

            try
            {
                var authTest = await gitHubClient.RateLimit.GetRateLimits(); // have to be authorised to access this. Thus, provides a test.                  
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        internal async Task<IReadOnlyList<Gist>> GetAllGistsAsync()
        {
            var gists = await gitHubClient.Gist.GetAll();
            return gists;
        }
        internal async Task<Gist> GetGistAsync(string gistID)
        {
            var gist = await gitHubClient.Gist.Get(gistID);
            return gist;
        }
        internal async Task<bool> GistIsStarredAsync(string gistId)
        {
            return await gitHubClient.Gist.IsStarred(gistId);
        }

        internal async Task<bool> SetGistIsStarredAsync(string gistId, bool isStarred)
        {
            if (isStarred)  await gitHubClient.Gist.Star(gistId);
            else await gitHubClient.Gist.Unstar(gistId);
            return true;
        }
        internal async Task<Gist> EditGistAsync(string gistId, GistUpdate gistUpdate)
        {
            Gist gist = await gitHubClient.Gist.Edit(gistId, gistUpdate);
            return gist;
        }
        internal async Task<Gist> CreateNewGistAsync(bool @public, string gistDescription, string gistFileFilename, string gistFileContents = null)
        {
            NewGist newGist = new NewGist()
            { Public = @public, Description = gistDescription };
            newGist.Files.Add(Helpers.String.MakeStringFilenameSafe(gistFileFilename), gistFileContents);
            Gist gist = await gitHubClient.Gist.Create(newGist);
            return gist;
        }
        internal async Task DeleteGistAsync(string gistId)
        {
            await gitHubClient.Gist.Delete(gistId);
        }
        internal async Task DoTestActionAsync(object obj)
        {
            var user = await gitHubClient.User.Get("stigzler");

            Debug.WriteLine("{0} has {1} public repositories - go check out their profile at {2}",
                user.Name,
                user.PublicRepos,
                user.Url);

            var gists = await gitHubClient.Gist.GetAllForUser("stigzler");
        }


        //internal async Task<bool> CanAuthenticate()
        //{

        //}


    }
}
