using Octokit;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace VisGist
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private async void TestBT_Click(object sender, RoutedEventArgs e)
        {
            var client = new GitHubClient(new ProductHeaderValue("VisGit-Tests"));
            var tokenAuth = new Credentials("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"); // NOTE: not real token
            client.Credentials = tokenAuth;

            var user = await client.User.Get("stigzler");

            Debug.WriteLine("{0} has {1} public repositories - go check out their profile at {2}",
                user.Name,
                user.PublicRepos,
                user.Url);


            var gists = await client.Gist.GetAllForUser("stigzler");


        }
    }
}