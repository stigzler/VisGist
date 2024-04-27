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
        [Description("An informative description.")]
        [DefaultValue(true)]
        //[TypeConverter(typeof(PatSettingConverter))]
        [PasswordPropertyText(true)]
        public string PersonalAccessToken
        {
            get => DpapiEncrypt.ToInsecureString(DpapiEncrypt.DecryptString(personalAccessToken));
            set => personalAccessToken = DpapiEncrypt.EncryptString(DpapiEncrypt.ToSecureString(value));
        }
    }
}
