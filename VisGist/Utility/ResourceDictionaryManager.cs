using System.Windows;
using System.Windows.Controls;

namespace VisGist.Utility
{
    public class ResourceDictionaryManager
    {
        public ResourceDictionary CurrentThemeResourceDictionary { get; set; }

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