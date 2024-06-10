using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGist.Helpers
{
    internal static class Logger
    {


        internal static void Log(string message)
        {
            string logFile = @"C:\temp\VisGist.log";
            System.IO.File.AppendAllText(logFile, message + Environment.NewLine);
        }

        internal static void Initialise()
        {
            string logFile = @"C:\temp\VisGist.log";
            System.IO.File.WriteAllText(logFile, string.Empty);
            Log($"New log started: {DateTime.Now}");            
         
        }



    }
}
