using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGist.Data.Models;

namespace VisGist.ViewModels
{
    internal class SyntaxViewModel
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public List<string> Extensions { get; set; } = new List<string>();

        public SyntaxViewModel() { }
        public SyntaxViewModel(Syntax syntax, bool useDarkThemedSyntax)
        {
            Name = syntax.Name;
            Extensions = new List<string>();
            if (useDarkThemedSyntax && syntax.FileDarkTheme != null) Filename = syntax.FileDarkTheme;
            else if (!useDarkThemedSyntax && syntax.FileLightTheme != null) Filename = syntax.FileLightTheme;
        }


    }
}
