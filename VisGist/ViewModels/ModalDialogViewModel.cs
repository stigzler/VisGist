using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using VisGist.Views;

namespace VisGist.ViewModels
{
    public class ModalDialogViewModel : ViewModelBase

    {
        #region Properties =========================================================================================

        //  ==============================================================================================
        //  Backing Vars
        //  ==============================================================================================

        private string _dialogText = "Dialog Text here";
        private string _button1Text = "Button 1";
        private string _button2Text = "Button 2";
        private string _selectedButtonText = null;
        private string _windowTitle = null;

        //  ==============================================================================================
        //  Public Members
        //  ==============================================================================================

        public string DialogText { get => _dialogText; set => SetProperty(ref _dialogText, value); }
        public string Button1Text { get => _button1Text; set => SetProperty(ref _button1Text, value); }
        public string Button2Text { get => _button2Text; set => SetProperty(ref _button2Text, value); }
        public string SelectedButtonText { get => _selectedButtonText; set => SetProperty(ref _selectedButtonText, value); }
        public string WindowTitle { get => _windowTitle; set => SetProperty(ref _windowTitle, value); }

        #endregion Properties =========================================================================================

        #region Commands =========================================================================================

        public ICommand ProcessButton1CMD { get; set; }
        public ICommand ProcessButton2CMD { get; set; }

        private void SetupCommandMethods()
        {
            ProcessButton1CMD = new RelayCommand<ModalDialogBase>(ProcessButton1);
            ProcessButton2CMD = new RelayCommand<ModalDialogBase>(ProcessButton2);
        }

        private void ProcessButton1(ModalDialogBase modalDialog)
        {
            SelectedButtonText = Button1Text;
            modalDialog.Close();
        }

        private void ProcessButton2(ModalDialogBase modalDialog)
        {
            SelectedButtonText = Button2Text;
            modalDialog.Close();
        }

        #endregion Commands =========================================================================================

        public ModalDialogViewModel()
        {
            SetupCommandMethods();
        }
    }
}