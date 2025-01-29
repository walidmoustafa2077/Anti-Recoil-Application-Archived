using Anti_Recoil_Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class ErrorDialogViewModel : ViewModelBase
    {
        private string _headerText = "Default Header";
        private string _buttonText = "Submit";
        private string _secondButtonText = string.Empty;

        public ErrorDialogViewModel(Action onButtonClick, Action? onSecondButtonClick = null)
        {
            ButtonCommand = new CommandBase(_ => onButtonClick?.Invoke());
            SecondButtonCommand = new CommandBase(_ => onSecondButtonClick?.Invoke());
        }

        public string HeaderText
        {
            get => _headerText;
            set => SetProperty(ref _headerText, value, nameof(HeaderText));
        }

        public string ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, value, nameof(ButtonText));
        }

        public string SecondButtonText
        {
            get => _secondButtonText;
            set
            {
                SetProperty(ref _secondButtonText, value, nameof(SecondButtonText));
                OnPropertyChanged(nameof(IsSecondButtonVisible));
            }
        }

        public Visibility IsSecondButtonVisible => string.IsNullOrEmpty(SecondButtonText) ? Visibility.Collapsed : Visibility.Visible;

        public ICommand ButtonCommand { get; }
        public ICommand SecondButtonCommand { get; }

    }
}
