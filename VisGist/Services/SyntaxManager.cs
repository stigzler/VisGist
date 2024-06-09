using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VisGist.Data.Models;

namespace VisGist.Services
{
    internal partial class SyntaxManager : ObservableObject
    {
        internal List<Syntax> _syntaxes = new List<Syntax>();
        public List<Syntax> Syntaxes { get => _syntaxes; set => SetProperty(ref _syntaxes, value); }

        internal SyntaxManager()
        {
            PopulateSyntaxes();
        }

        internal void PopulateSyntaxes()
        {
            string lightThemedSyntaxDir = Path.Combine(Data.Constants.UserSyntaxDirectory, "Light");
            string darkThemedSyntaxDir = Path.Combine(Data.Constants.UserSyntaxDirectory, "Dark");

            // Iterate trough Light Theme Syntax files to add Syntax defs to Syntaxes ObsCollection
            foreach (string syntaxFile in Directory.GetFiles(lightThemedSyntaxDir))
            {
                XDocument xmlDoc = XDocument.Load(syntaxFile);

                Syntax newSyntax = new Syntax()
                {
                    Name = xmlDoc.Root.Attribute("name").Value,
                    Extensions = xmlDoc.Root.Attribute("extensions")?.Value.Split(';').ToList(),
                    FileLightTheme = syntaxFile
                };

                Syntaxes.Add(newSyntax);
            }

            // Iterate trough Dark Theme Syntax files to either update an existing Syntax definition or insert a new one.
            foreach (string syntaxFile in Directory.GetFiles(darkThemedSyntaxDir))
            {
                XDocument xmlDoc = XDocument.Load(syntaxFile);

                Syntax matchedLightSyntax = Syntaxes.Where(s => s.Name == xmlDoc.Root.Attribute("name").Value).FirstOrDefault();

                if (matchedLightSyntax != null)
                {
                    matchedLightSyntax.FileDarkTheme = syntaxFile;
                }
                else
                {
                    Syntax newSyntax = new Syntax()
                    {
                        Name = xmlDoc.Root.Attribute("name").Value,
                        Extensions = xmlDoc.Root.Attribute("extensions")?.Value.Split(';').ToList(),
                        FileDarkTheme = syntaxFile
                    };
                    Syntaxes.Add(newSyntax);
                }
            }

            Syntaxes = Syntaxes.OrderBy(s => s.Name).ToList();
        }
    }
}