using Anti_Recoil_Application.Commands;
using System.Windows;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class ErrorDialogViewModel : MainDialogViewModel
    {
        private string _secondButtonText = string.Empty;

        public ErrorDialogViewModel(Action onButtonClick, Action? onSecondButtonClick = null)
        : base(onButtonClick)
        {
            SecondButtonCommand = new CommandBase(_ => onSecondButtonClick?.Invoke());
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

        public ICommand SecondButtonCommand { get; }

    }
}
