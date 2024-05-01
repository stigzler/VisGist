using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VisGist.ViewModels;

namespace VisGist.Converters
{
    internal class GistFilesVMToFirstFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            BindingList<GistFileViewModel> gistFiles = (BindingList<GistFileViewModel>) value;
            if (gistFiles.Count > 0)
            {
                return gistFiles.OrderBy(gf => gf.Filename).First().Filename;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
