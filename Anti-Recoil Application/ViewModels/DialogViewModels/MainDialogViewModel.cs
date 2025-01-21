using Anti_Recoil_Application.Commands;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class MainDialogViewModel : ViewModelBase
    {
        private string _headerText = "Default Header";
        private string _buttonText = "Submit";

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

        public ICommand ButtonCommand { get; }

        public MainDialogViewModel(Action onButtonClick)
        {
            ButtonCommand = new CommandBase(_ => onButtonClick?.Invoke());
        }

    }
}
