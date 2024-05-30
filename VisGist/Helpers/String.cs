using Microsoft.VisualStudio.VCProjectEngine;
using Newtonsoft.Json.Linq;
using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Helpers
{
    internal static class String
    {
        internal static string MakeStringFilenameSafe(string str, char replacemwentChar = '-')
        {
            string retStr = str;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                retStr = retStr.Replace(c, replacemwentChar);
            }
            return retStr;
        }

        internal static bool ContainsIllegalFilenameChars(string str)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (str.Contains(c)) return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">filename with Extension</param>
        /// <returns>Matched Language or Languages.Text if no match found</returns>
        internal static Languages FilenameToLanguage(string filename)
        {
            string ext = Path.GetExtension(filename).Replace(".", "");

            if (ext != null)
            {
                var languageKvp = Data.Constants.CodeLanguageMappings.Where(x => x.Key.Contains(ext)).FirstOrDefault();
                if (!languageKvp.Equals(default(KeyValuePair<List<string>, Languages>)))
                {
                    return languageKvp.Value;
                }
            }
            return Languages.Text;
        }

    }
}
