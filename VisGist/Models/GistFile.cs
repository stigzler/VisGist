﻿namespace VisGist.Models
{
    internal class GistFile
    {
        public Octokit.GistFile ImportedGistFile { get; set; }

        public string Content { get; set; }

        public string Filename { get; set; }

        public string Language { get; set; }

        public GistFile(Octokit.GistFile gistFile)
        {
            ImportedGistFile = gistFile;
            Content = ImportedGistFile.Content;
            Filename = ImportedGistFile.Filename;
            Language = ImportedGistFile.Language;
        }
    }
}