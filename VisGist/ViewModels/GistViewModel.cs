﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisGist.ViewModels
{
    internal class GistViewModel : ViewModelBase, IDisposable
    {
        #region Private Backing Vars =========================================================================================

        private BindingList<GistFileViewModel> gistFiles = new();
        private bool starred = false;
        private bool @public = false;
        private string description = string.Empty;


        private bool hasChanges = false;
        private bool canSave = true;
        private string propertyChanged = string.Empty;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;




        #endregion End: Private Backing Vars

        #region Properties =========================================================================================

        /// <summary>
        /// Github Gist ID
        /// </summary>
        public string Id { get => ReferenceGist.Id; }

        /// <summary>
        /// Constructs the FirendlyName from the filename of the alphabetically sorted GistFiles list
        /// </summary>
        public string PrimaryGistFilename
        {
            get
            {
                if (GistFiles.Count == 0) return null;
                return GistFiles.OrderBy(gf => gf.Filename).First().Filename;
            }
        }

        /// <summary>
        /// This is either the Gist as imported from Github, or the updated Gist following Gist Save
        /// Used in tracking changes
        /// </summary>
        public Octokit.Gist ReferenceGist { get; set; }

        /// <summary>
        /// Gists does not store 'Starred' as a property in Octocat.Gist. Thus,
        /// have to monitor it state separately. 
        /// </summary>
        public bool ReferenceStarred { get; set; }

        /// <summary>
        /// Whether Gist has changes compared to vanilla Imported Gist, or
        /// since last save. Also, whether any child GistFile has changes. Leverages ReferenceGist
        /// </summary>
        public bool HasChanges { get => hasChanges; set => SetProperty(ref hasChanges, value); }

        public bool CanSave { get => canSave; set => SetProperty(ref canSave, value); }

        public bool HasErrors => gistFiles.Any(gf => gf.HasErrors);

 

        /// <summary>
        /// Holds a string describing which properties have changed. Can be used
        /// with Tooltips etc.
        /// </summary>
        public string PropertiesChanged { get => propertyChanged; set => SetProperty(ref propertyChanged,value); }

        public string Description
        {
            get { return description; }
            set
            {
                SetProperty(ref description, value);
                HasChanges = GistHasChanges();

            }
        }

        public bool Public
        {
            get { return @public; }
            set
            {
                SetProperty(ref @public, value);
                HasChanges = GistHasChanges();

            }
        }

        public bool Starred
        {
            get { return starred; }
            set
            {
                SetProperty(ref starred, value);
                HasChanges = GistHasChanges();

            }
        }

        public BindingList<GistFileViewModel> GistFiles { get => gistFiles; set => SetProperty(ref gistFiles, value); }


        #endregion End: Properties



        #region Commands =========================================================================================



        private void RegisterCommands()
        {
        }


        private async Task SaveGistAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }





        #endregion End: Commands


        #region Constructors =========================================================================================

        public GistViewModel(Octokit.Gist gist)
        {
            ReferenceGist = gist;
            description = gist.Description;
            @public = gist.Public;

            foreach (KeyValuePair<string, Octokit.GistFile> fileKvp in gist.Files)
            {
                GistFiles.Add(new GistFileViewModel(fileKvp.Value, this));
            }

            GistFiles.ListChanged += GistFiles_ListChanged;

            RegisterCommands();
        }

        #endregion End: Constructors


        #region Private Methods =========================================================================================

        // OPERATIONAL ==============================================================================================

        private void GistFiles_ListChanged(object sender, ListChangedEventArgs e)
        {
            GistFiles = new BindingList<GistFileViewModel>(GistFiles.OrderBy(gf => gf.Filename).ToList());
            OnPropertyChanged(nameof(GistFiles));
        }

        internal bool GistHasChanges()
        {
            bool hasChanges = false;

            PropertiesChanged = Data.Constants.GistPropertiesChangedPrefix;

            if (starred != ReferenceStarred) { hasChanges = true; PropertiesChanged += "Starred, "; }
            if (Public != ReferenceGist.Public) { hasChanges = true; PropertiesChanged += "Public, "; }
            if (Description != ReferenceGist.Description) { hasChanges = true; PropertiesChanged += "Description, "; }
            if (GistFiles.Any(gf => gf.HasChanges)) { hasChanges = true; PropertiesChanged += "File/s, "; }

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

        #endregion End: Private Methods



        public void Dispose()
        {
            GistFiles.ListChanged -= GistFiles_ListChanged;
        }


    }
}
