﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGist.Converters
{
    internal class GistFilesToFirstFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ObservableCollection<Models.GistFile> gistFiles = (ObservableCollection<Models.GistFile>) value;
            if (gistFiles.Count > 0)
            {
                return gistFiles.First().Filename;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
