﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }
}
