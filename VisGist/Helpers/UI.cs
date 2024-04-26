using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Helpers
{
    internal static class UI
    {

        /// <summary>
        /// Sure there's a better way to do this, but meh
        /// Examines VS's ToolWindowBackgroundColorKey. If Red lower than 100 (arbitrary) return dark mode is true
        /// Red due to BGs RGB values all being same(ish?)
        /// </summary>
        /// <returns></returns>
        internal static bool IsDarkMode()
        {
            var defaultBackground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            if (defaultBackground.R < 100) return true;
            return false;
        }

    }
}
