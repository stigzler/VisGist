using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Data.Models
{
    internal class Syntax
    {
        public string Name { get; set; }
        public List<string> Extensions { get; set; } = new List<string>();
        public string FileLightTheme { get; set; } = null;
        public string FileDarkTheme { get; set; } = null;
    }
}
