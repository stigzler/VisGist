using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Services
{
    internal class CodeEditorManager
    {

        public EditControl EditControl { get; set; }

        public CodeEditorManager(EditControl editControl)
        {
            EditControl = editControl;
        }



    }
}
