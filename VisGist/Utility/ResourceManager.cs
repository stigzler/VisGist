using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisGist.Utility
{
    public class ResourceManager
    {

        public ResourceDictionary CurrentThemeResourceDictionary {  get; set; }


        internal void ChangeTheme(Uri themeUri, UserControl userControl)
        {
            ResourceDictionary themeResourceDictionary = new ResourceDictionary() { Source = themeUri };

            if (userControl.Resources.MergedDictionaries.Contains(CurrentThemeResourceDictionary))
            {
                userControl.Resources.MergedDictionaries.Remove(CurrentThemeResourceDictionary);
            }

            userControl.Resources.MergedDictionaries.Add(themeResourceDictionary);
            CurrentThemeResourceDictionary = themeResourceDictionary;
        }



    }
}
