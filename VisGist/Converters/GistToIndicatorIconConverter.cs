using Microsoft.VisualStudio.Imaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VisGist.ViewModels;

namespace VisGist.Converters
{
    internal class GistToIndicatorIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool canSave = (bool)values[0];
            bool hasChanges = (bool)values[1];

            if (!canSave) return KnownMonikers.ExclamationPoint; 
            else if (hasChanges) return KnownMonikers.Save; 
            else return KnownMonikers.Item;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
