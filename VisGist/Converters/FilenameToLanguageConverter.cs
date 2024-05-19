using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGist.Converters
{
    internal class FilenameToLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filename = (string)value;
            string ext = Path.GetExtension(filename).Replace(".", "");

            var dave = parameter;

            if (ext != null)
            {
                var languageKvp = Data.
                    Constants.CodeLanguageMappings.Where(x => x.Key.Contains(ext)).FirstOrDefault();
                if (!languageKvp.Equals(default(KeyValuePair<List<string>, Languages>)))
                {
                    return languageKvp.Value;
                }
            }

            return Languages.Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
