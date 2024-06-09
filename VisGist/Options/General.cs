using System.ComponentModel;
using System.Runtime.InteropServices;
using VisGist.Enums;
using VisGist.Helpers;

namespace VisGist
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "VisGist", "General", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General>
        { }
    }

    public class General : BaseOptionModel<General>
    {
        private string personalAccessToken = "{unset}";

        [Category("Github Settings")]
        [DisplayName("Personal Access Token")]
        [Description("Your GitHub Personal Access Token. Get one via Github > Settings > Developer Settings > Personal Access Tokens > Fine-grained tokens")]
        [DefaultValue(true)]
        [PasswordPropertyText(true)]
        public string PersonalAccessToken
        {
            get => DpapiEncrypt.ToInsecureString(DpapiEncrypt.DecryptString(personalAccessToken));
            set => personalAccessToken = DpapiEncrypt.EncryptString(DpapiEncrypt.ToSecureString(value));
        }

        [Category("General")]
        [DisplayName("New Gists Public")]
        [Description("Sets whether to default newly created Gists to Public visibility. If false, new Gists will be private. Warning: Private Gists can be set to Public, but not vice-versa.")]
        [DefaultValue(false)]
        public bool NewGistPublic { get; set; } = false;

        [Category("General")]
        [DisplayName("Auto Language Select")]
        [Description("Tries to automatically select the code language from the file extension")]
        [DefaultValue(false)]
        public bool AutoLanguageSelect { get; set; } = false;

        [Category("General")]
        [DisplayName("Confirm Delete")]
        [Description("Asks for confirmation when deleting items.")]
        [DefaultValue(true)]
        public bool ConfirmDelete { get; set; } = true;

        [Category("General")]
        [DisplayName("Gist File Heading")]
        [Description("Chooses which character to use as the Heading character.")]
        public GistFileHeadingChar GistFileHeadingCharacter { get; set; } = GistFileHeadingChar.Dot;
    }
}