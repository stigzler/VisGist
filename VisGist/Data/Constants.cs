using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGist.Enums;

namespace VisGist.Data
{
    internal static class Constants
    {
        internal const string GitProductHeaderValue = "VisGist";
        internal const string GistPropertiesChangedPrefix = "Gist Changes: ";
        internal const string GistNoPropertiesChangedPrefix = "No Changes";

        internal static readonly string UserSyntaxDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VisGist\\Highlighting");
        internal static readonly string DefaultSyntaxDirectory = Path.Combine(Path.GetDirectoryName(  System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources\\Highlighting");

        //internal static readonly Dictionary<List<String>, Languages> CodeLanguageMappings = new Dictionary<List<string>, Languages>()
        //    {
        //    {new List<string>() {"c" }, Languages.C },
        //    {new List<string>() {"cs" }, Languages.CSharp },
        //    {new List<string>() {"dpr", "pas", "dfm" }, Languages.Delphi },
        //    {new List<string>() {"html", "htm" }, Languages.HTML },
        //    {new List<string>() {"java" }, Languages.Java },
        //    {new List<string>() {"js", "cjs", "mjs" }, Languages.JScript },
        //    {new List<string>() {"ps1" }, Languages.PowerShell },
        //    {new List<string>() {"sql" }, Languages.SQL },
        //    {new List<string>() {"txt" }, Languages.Text },
        //    {new List<string>() {"vbs" }, Languages.VBScript },
        //    {new List<string>() {"vb" }, Languages.VisualBasic },
        //    {new List<string>() {"xaml" }, Languages.XAML },
        //    {new List<string>() {"xml" }, Languages.XML },
        //};

        internal static readonly Dictionary<GistFileHeadingChar, char> GistFileHeadingChars = new Dictionary<GistFileHeadingChar, char>()
        {
            {GistFileHeadingChar.Dot, '.' },
            {GistFileHeadingChar.Exclamation , '!' },
            {GistFileHeadingChar.Hash, '#' },
            {GistFileHeadingChar.Minus, '-' }
        };


    }
}
