using System.Collections.Generic;
using System.IO;
using VisGist.Enums;

namespace VisGist.Data
{
    internal static class Constants
    {
        internal const string GitProductHeaderValue = "VisGist";
        internal const string GistPropertiesChangedPrefix = "Gist Changes: ";
        internal const string GistNoPropertiesChangedPrefix = "No Changes";

        internal static readonly string UserSyntaxDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VisGist\\Highlighting");
        internal static readonly string DefaultSyntaxDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources\\Highlighting");

        internal static readonly Dictionary<GistFileHeadingChar, char> GistFileHeadingChars = new Dictionary<GistFileHeadingChar, char>()
        {
            {GistFileHeadingChar.Dot, '.' },
            {GistFileHeadingChar.Exclamation , '!' },
            {GistFileHeadingChar.Hash, '#' },
            {GistFileHeadingChar.Minus, '-' }
        };
    }
}