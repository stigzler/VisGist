using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.VisualStudio.VSConstants;

namespace VisGist.TypeConverters
{
    internal class PatSettingConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType != typeof(string)) { return false; }
            return true;
            // return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var cast = value as string;
            return cast != null
                ? Helpers.DpapiEncrypt.ToSecureString(cast)
                : base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {

            var pat = value as string;

            return $"[ends in: {pat.Substring(pat.Length - 5)}]";
        }





    }
}
