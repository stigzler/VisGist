using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGist.Data.Models;

namespace VisGist.Services
{
    internal partial class SyntaxManager: ObservableObject
    {
        internal List<Syntax> _syntaxes = new List<Syntax> ();
        public List<Syntax> Syntaxes { get => _syntaxes; set => SetProperty(ref _syntaxes, value); }
        internal SyntaxManager()
        {
            PopulateSyntaxes();
        }

        internal void PopulateSyntaxes()
        {
            string lightThemedSyntaxDir = Path.Combine(Data.Constants.UserSyntaxDirectory, "Light");
            string darkThemedSyntaxDir = Path.Combine(Data.Constants.UserSyntaxDirectory, "Dark");

            //ObservableCollection<Syntax> unsortedSyntaxes = new ObservableCollection<Syntax> ();

            // Iterate trough Light Theme Syntax files to add Syntax defs to Syntaxes ObsCollection
            foreach (string syntaxFile in Directory.GetFiles(lightThemedSyntaxDir))
            {
                // Extract syntax definition line from xml
                var definitionLine = File.ReadLines(syntaxFile).Where(line => line.StartsWith("<SyntaxDefinition")).FirstOrDefault();
                
                // delineate Attributes
                var delineatedParts = definitionLine.Split('"');

                Syntax newSyntax = new Syntax()
                {
                    Name = delineatedParts[1],
                    Extensions = delineatedParts[2].Split(';').ToList(),
                    FileLightTheme = syntaxFile
                };
                Syntaxes.Add(newSyntax);
            }

            // Iterate trough Dark Theme Syntax files to either update an existing Syntax definition or insert a new one.
            foreach (string syntaxFile in Directory.GetFiles(darkThemedSyntaxDir))
            {
                // Extract syntax definition line from xml
                var definitionLine = File.ReadLines(syntaxFile).Where(line => line.StartsWith("<SyntaxDefinition")).FirstOrDefault();

                // delineate Attributes
                var delineatedParts = definitionLine.Split('"');

                Syntax matchedLightSyntax = Syntaxes.Where(s => s.Name == delineatedParts[1]).FirstOrDefault();

                if (matchedLightSyntax != null)
                {
                    matchedLightSyntax.FileDarkTheme = syntaxFile;
                }
                else
                {
                    Syntax newSyntax = new Syntax()
                    {
                        Name = delineatedParts[1],
                        Extensions = delineatedParts[2].Split(';').ToList(),
                        FileDarkTheme = syntaxFile
                    };
                    Syntaxes.Add(newSyntax);
                }
            }

            Syntaxes.Count();
        }


    }
}
