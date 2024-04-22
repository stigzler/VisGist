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
            var client = new GitHubClient(new ProductHeaderValue("stigzler"));
            var tokenAuth = new Credentials("408da1d19d6fc7a5006e56a843a3da99fe365a34"); // NOTE: not real token
            client.Credentials = tokenAuth;

            var user = await client.User.Get("stigzler");
            Debug.WriteLine("{0} has {1} public repositories - go check out their profile at {2}",
                user.Name,
                user.PublicRepos,
                user.Url);
        }
    }
}