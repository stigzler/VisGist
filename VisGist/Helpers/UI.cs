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
        /// Returns whether Visual Studio is in DarkMode
        /// Arbitrary method: returns true if the Red value of background is lower than 100
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
