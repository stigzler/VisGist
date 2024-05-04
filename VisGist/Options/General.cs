using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using VisGist.Helpers;
using VisGist.TypeConverters;

namespace VisGist
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "VisGist", "General", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        private string personalAccessToken = "{unset}";


        [Category("Github Settings")]
        [DisplayName("Personal Access Token")]
        [Description("Your GitHub Personal Access Token. Get one via Github > Settings > Developer Settings > Personal Access Tokens > Fine-grained tokens")]
        [DefaultValue(true)]
        //[TypeConverter(typeof(PatSettingConverter))]
        [PasswordPropertyText(true)]
        public string PersonalAccessToken
        {
            get => DpapiEncrypt.ToInsecureString(DpapiEncrypt.DecryptString(personalAccessToken));
            set => personalAccessToken = DpapiEncrypt.EncryptString(DpapiEncrypt.ToSecureString(value));
        }


        [Category("Github Settings")]
        [DisplayName("Auto Log-In")]
        [Description("Signs you into VisGist at startup")]
        [DefaultValue(false)]
        public bool AutoLogin { get; set; }


        [Category("Github Settings")]
        [DisplayName("New Gists Public")]
        [Description("Sets whether to default newly created Gists to Public visibility. If false, new Gists will be private.")]
        [DefaultValue(true)]
        public bool NewGistPublic { get; set; }


    }
}
