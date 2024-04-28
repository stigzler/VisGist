using Octokit;
using System;
using System.Collections.Generic;
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
