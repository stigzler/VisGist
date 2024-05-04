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
            foreach(char c in Path.GetInvalidFileNameChars())
            {
                retStr = retStr.Replace(c, replacemwentChar);
            }
            return retStr;
        }
    }
}
