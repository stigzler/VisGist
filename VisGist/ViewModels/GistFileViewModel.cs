using Microsoft.VisualStudio.VCProjectEngine;
using Syncfusion.Windows.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VisGist.Extensions;

namespace VisGist.ViewModels
{
    internal class GistFileViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private string filename;
        private string content;

        private bool hasChanges;

        private string propertyChanged = string.Empty;
        private string lastUniqueFilename;
        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// This is either the Gist imported from Gists, or the updated Gist following Gist Save
        /// Used in tracking changes
        /// </summary>
        public Octokit.GistFile ReferenceGistFile { get; set; }
        public GistViewModel ParentGistViewModel { get; set; }
        public bool HasChanges { get => hasChanges; set => SetProperty(ref hasChanges, value); }
        public bool HasErrors => propertyErrors.Any();
        public bool MarkedForDeletion { get; set; } = false;

        /// <summary>
        /// Holds a string describing which properties have changed. Can be used
        /// with Tooltips etc.
        /// </summary>
        public string PropertiesChanged { get => propertyChanged; set => SetProperty(ref propertyChanged, value); }


        // GistFile mirrors
        public string Id = Guid.NewGuid().ToString();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public string Content {
            get { return  content; }
            set
            {
                ClearErrors(nameof(Content));
                //if (value == null || value.Length == 0 || value == String.Empty || value=="\r\n\r\n" || value == "\r\n")
                //{
                //    AddError(nameof(Content), "The Code cannot be null");
                //    ParentGistViewModel.CanSave = false;
                //}
                //else
                //{
                //    ParentGistViewModel.CanSave = true;
                //}

                if (value != ReferenceGistFile.Content) HasChanges = true;
                else HasChanges = false;

                SetProperty(ref content, value);
                ParentGistViewModel.HasChanges = ParentGistViewModel.GistHasChanges();
            }
        }

        public string Filename
        {
            get { return filename; }
            set
            {
                ClearErrors(nameof(Filename));

                if (value == "" || value == null || value == String.Empty)
                {
                    AddError(nameof(Filename), "The Filename cannot be null");
                    ParentGistViewModel.CanSave = false;
                }
                else if (!IsUniqueFilename(value))
                {
                    AddError(nameof(Filename), "Filename must be unique for the Gist");
                    ParentGistViewModel.CanSave = false;
                }
                else if (Helpers.String.ContainsIllegalFilenameChars(value))
                {
                    AddError(nameof(Filename), "Filename contains illegal characters");
                    ParentGistViewModel.CanSave = false;
                }
                else
                {
                    ParentGistViewModel.CanSave = true;
                }

                if (value != ReferenceGistFile.Filename) HasChanges = true;
                else HasChanges = false;

                SetProperty(ref filename, value);
                base.OnPropertyChanged(nameof(Filename));
                ParentGistViewModel.HasChanges = ParentGistViewModel.GistHasChanges();
            }
        }

        public string LastUniqueFilename { get => lastUniqueFilename; }

        public GistFileViewModel(Octokit.GistFile gistFile, GistViewModel parentGistViewModel)
        {
            ReferenceGistFile = gistFile;
            ParentGistViewModel = parentGistViewModel;
            Content = ReferenceGistFile.Content;
            Filename = ReferenceGistFile.Filename;
            lastUniqueFilename = Filename;
        }

        internal bool IsUniqueFilename(string candidateFilename)
        {
            // Tests Gists files for matching Filename and also excludes itself form the comparisons
            if (ParentGistViewModel.GistFiles.Any(gf => gf.Id != Id && gf.Filename.ToLower() == candidateFilename.ToLower()))
            {
                return false;
            }
            lastUniqueFilename = candidateFilename;
            return true;
        }

        private bool GistFileHasChanges()
        {
            bool hasChanges = false;

            PropertiesChanged = Data.Constants.GistPropertiesChangedPrefix;

            if (Filename != ReferenceGistFile.Filename) { hasChanges = true; PropertiesChanged += "Filename, "; }
            if (Content != ReferenceGistFile.Content) { hasChanges = true; PropertiesChanged += "Code, "; }

            // if items have been added, remove the trailing ", "
            if (PropertiesChanged != Data.Constants.GistPropertiesChangedPrefix)
            {
                PropertiesChanged = PropertiesChanged.Substring(0, PropertiesChanged.Length - 2);
            }
            else
            {
                PropertiesChanged = Data.Constants.GistNoPropertiesChangedPrefix;
            }

            return hasChanges;
        }


        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null) return null;
            return propertyErrors.GetValueOrDefault(propertyName, null);
        }

        internal void ClearErrors(string propertyName)
        {
            if (propertyErrors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Add(propertyName, new List<string>());
            }
            propertyErrors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);

        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }


    }
}
