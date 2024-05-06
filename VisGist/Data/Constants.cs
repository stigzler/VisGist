﻿using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Data
{
    internal static class Constants
    {
        internal const string GitProductHeaderValue = "VisGist";
        internal const string GistPropertiesChangedPrefix = "Gist Changes: ";
        internal const string GistNoPropertiesChangedPrefix = "No Changes";

        internal static readonly Dictionary<List<String>, Languages> CodeLanguageMappings = new Dictionary<List<string>, Languages>()
            {
            {new List<string>() {"c" }, Languages.C },
            {new List<string>() {"cs" }, Languages.CSharp },
            {new List<string>() {"dpr", "pas", "dfm" }, Languages.Delphi },
            {new List<string>() {"html", "htm" }, Languages.HTML },
            {new List<string>() {"java" }, Languages.Java },
            {new List<string>() {"js", "cjs", "mjs" }, Languages.JScript },
            {new List<string>() {"ps1" }, Languages.PowerShell },
            {new List<string>() {"sql" }, Languages.SQL },
            {new List<string>() {"txt" }, Languages.Text },
            {new List<string>() {"vbs" }, Languages.VBScript },
            {new List<string>() {"vb" }, Languages.VisualBasic },
            {new List<string>() {"xaml" }, Languages.XAML },
            {new List<string>() {"xml" }, Languages.XML },
        };

    }
}
